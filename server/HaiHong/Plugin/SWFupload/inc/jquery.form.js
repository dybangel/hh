/*!
 * jQuery Form Plugin
 * version: 3.36.0-2013.06.16
 * @requires jQuery v1.5 or later
 * Copyright (c) 2013 M. Alsup
 * Examples and documentation at: http://malsup.com/jquery/form/
 * Project repository: https://github.com/malsup/form
 * Dual licensed under the MIT and GPL licenses.
 * https://github.com/malsup/form#copyright-and-license
 */
/*global ActiveXObject */
;(function($) {
"use strict";

/*
    Usage Note:
    -----------
    Do not use both ajaxSubmit and ajaxForm on the same form.  These
    functions are mutually exclusive.  Use ajaxSubmit if you want
    to bind your own submit handler to the form.  For example,

    $(document).ready(function() {
        $('#myForm').on('submit', function(e) {
            e.preventDefault(); // <-- important
            $(this).ajaxSubmit({
                target: '#output'
            });
        });
    });

    Use ajaxForm when you want the plugin to manage all the event binding
    for you.  For example,

    $(document).ready(function() {
        $('#myForm').ajaxForm({
            target: '#output'
        });
    });

    You can also use ajaxForm with delegation (requires jQuery v1.7+), so the
    form does not have to exist when you invoke ajaxForm:

    $('#myForm').ajaxForm({
        delegation: true,
        target: '#output'
    });

    When using ajaxForm, the ajaxSubmit function will be invoked for you
    at the appropriate time.
*/

/**
 * Feature detection
 */
var feature = {};
feature.fileapi = $("<input type='file'/>").get(0).files !== undefined;
feature.formdata = window.FormData !== undefined;

var hasProp = !!$.fn.prop;

// attr2 uses prop when it can but checks the return type for
// an expected string.  this accounts for the case where a form 
// contains inputs with names like "action" or "method"; in those
// cases "prop" returns the element
$.fn.attr2 = function() {
    if ( ! hasProp )
        return this.attr.apply(this, arguments);
    var val = this.prop.apply(this, arguments);
    if ( ( val && val.jquery ) || typeof val === 'string' )
        return val;
    return this.attr.apply(this, arguments);
};

/**
 * ajaxSubmit() provides a mechanism for immediately submitting
 * an HTML form using AJAX.
 */
$.fn.ajaxSubmit = function(options) {
    /*jshint scripturl:true */

    // fast fail if nothing selected (http://dev.jquery.com/ticket/2752)
    if (!this.length) {
        log('ajaxSubmit: skipping submit process - no element selected');
        return this;
    }

    var method, action, url, $form = this;

    if (typeof options == 'function') {
        options = { success: options };
    }

    method = options.type || this.attr2('method');
    action = options.url  || this.attr2('action');

    url = (typeof action === 'string') ? $.trim(action) : '';
    url = url || window.location.href || '';
    if (url) {
        // clean url (don't include hash vaue)
        url = (url.match(/^([^#]+)/)||[])[1];
    }

    options = $.extend(true, {
        url:  url,
        success: $.ajaxSettings.success,
        type: method || 'GET',
        iframeSrc: /^https/i.test(window.location.href || '') ? 'javascript:false' : 'about:blank'
    }, options);

    // hook for manipulating the form data before it is extracted;
    // convenient for use with rich editors like tinyMCE or FCKEditor
    var veto = {};
    this.trigger('form-pre-serialize', [this, options, veto]);
    if (veto.veto) {
        log('ajaxSubmit: submit vetoed via form-pre-serialize trigger');
        return this;
    }

    // provide opportunity to alter form data before it is serialized
    if (options.beforeSerialize && options.beforeSerialize(this, options) === false) {
        log('ajaxSubmit: submit aborted via beforeSerialize callback');
        return this;
    }

    var traditional = options.traditional;
    if ( traditional === undefined ) {
        traditional = $.ajaxSettings.traditional;
    }

    var elements = [];
    var qx, a = this.formToArray(options.semantic, elements);
    if (options.data) {
        options.extraData = options.data;
        qx = $.param(options.data, traditional);
    }

    // give pre-submit callback an opportunity to abort the submit
    if (options.beforeSubmit && options.beforeSubmit(a, this, options) === false) {
        log('ajaxSubmit: submit aborted via beforeSubmit callback');
        return this;
    }

    // fire vetoable 'validate' event
    this.trigger('form-submit-validate', [a, this, options, veto]);
    if (veto.veto) {
        log('ajaxSubmit: submit vetoed via form-submit-validate trigger');
        return this;
    }

    var q = $.param(a, traditional);
    if (qx) {
        q = ( q ? (q + '&' + qx) : qx );
    }
    if (options.type.toUpperCase() == 'GET') {
        options.url += (options.url.indexOf('?') >= 0 ? '&' : '?') + q;
        options.data = null;  // data is null for 'get'
    }
    else {
        options.data = q; // data is the query string for 'post'
    }

    var callbacks = [];
    if (options.resetForm) {
        callbacks.push(function() { $form.resetForm(); });
    }
    if (options.clearForm) {
        callbacks.push(function() { $form.clearForm(options.includeHidden); });
    }

    // perform a load on the target only if dataType is not provided
    if (!options.dataType && options.target) {
        var oldSuccess = options.success || function(){};
        callbacks.push(function(data) {
            var fn = options.replaceTarget ? 'replaceWith' : 'html';
            $(options.target)[fn](data).each(oldSuccess, arguments);
        });
    }
    else if (options.success) {
        callbacks.push(options.success);
    }

    options.success = function(data, status, xhr) { // jQuery 1.4+ passes xhr as 3rd arg
        var context = options.context || this ;    // jQuery 1.4+ supports scope context
        for (var i=0, max=callbacks.length; i < max; i++) {
            callbacks[i].apply(context, [data, status, xhr || $form, $form]);
        }
    };

    if (options.error) {
        var oldError = options.error;
        options.error = function(xhr, status, error) {
            var context = options.context || this;
            oldError.apply(context, [xhr, status, error, $form]);
        };
    }

     if (options.complete) {
        var oldComplete = options.complete;
        options.complete = function(xhr, status) {
            var context = options.context || this;
            oldComplete.apply(context, [xhr, status, $form]);
        };
    }

    // are there files to upload?

    // [value] (issue #113), also see comment:
    // https://github.com/malsup/form/commit/588306aedba1de01388032d5f42a60159eea9228#commitcomment-2180219
    var fileInputs = $('input[type=file]:enabled[value!=""]', this);

    var hasFileInputs = fileInputs.length > 0;
    var mp = 'multipart/form-data';
    var multipart = ($form.attr('enctype') == mp || $form.attr('encoding') == mp);

    var fileAPI = feature.fileapi && feature.formdata;
    log("fileAPI :" + fileAPI);
    var shouldUseFrame = (hasFileInputs || multipart) && !fileAPI;

    var jqxhr;

    // options.iframe allows user to force iframe mode
    // 06-NOV-09: now defaulting to iframe mode if file input is detected
    if (options.iframe !== false && (options.iframe || shouldUseFrame)) {
        // hack to fix Safari hang (thanks to Tim Molendijk for this)
        // see:  http://groups.google.com/group/jquery-dev/browse_thread/thread/36395b7ab510dd5d
        if (options.closeKeepAlive) {
            $.get(options.closeKeepAlive, function() {
                jqxhr = fileUploadIframe(a);
            });
        }
        else {
            jqxhr = fileUploadIframe(a);
        }
    }
    else if ((hasFileInputs || multipart) && fileAPI) {
        jqxhr = fileUploadXhr(a);
    }
    else {
        jqxhr = $.ajax(options);
    }

    $form.removeData('jqxhr').data('jqxhr', jqxhr);

    // clear element array
    for (var k=0; k < elements.length; k++)
        elements[k] = null;

    // fire 'notify' event
    this.trigger('form-submit-notify', [this, options]);
    return this;

    // utility fn for deep serialization
    function deepSerialize(extraData){
        var serialized = $.param(extraData, options.traditional).split('&');
        var len = serialized.length;
        var result = [];
        var i, part;
        for (i=0; i < len; i++) {
            // #252; undo param space replacement
            serialized[i] = serialized[i].replace(/\+/g,' ');
            part = serialized[i].split('=');
            // #278; use array instead of object storage, favoring array serializations
            result.push([decodeURIComponent(part[0]), decodeURIComponent(part[1])]);
        }
        return result;
    }

     // XMLHttpRequest Level 2 file uploads (big hat tip to francois2metz)
    function fileUploadXhr(a) {
        var formdata = new FormData();

        for (var i=0; i < a.length; i++) {
            formdata.append(a[i].name, a[i].value);
        }

        if (options.extraData) {
            var serializedData = deepSerialize(options.extraData);
            for (i=0; i < serializedData.length; i++)
                if (serializedData[i])
                    formdata.append(serializedData[i][0], serializedData[i][1]);
        }

        options.data = null;

        var s = $.extend(true, {}, $.ajaxSettings, options, {
            contentType: false,
            processData: false,
            cache: false,
            type: method || 'POST'
        });

        if (options.uploadProgress) {
            // workaround because jqXHR does not expose upload property
            s.xhr = function() {
                var xhr = $.ajaxSettings.xhr();
                if (xhr.upload) {
                    xhr.upload.addEventListener('progress', function(event) {
                        var percent = 0;
                        var position = event.loaded || event.position; /*event.position is deprecated*/
                        var total = event.total;
                        if (event.lengthComputable) {
                            percent = Math.ceil(position / total * 100);
                        }
                        options.uploadProgress(event, position, total, percent);
                    }, false);
                }
                return xhr;
            };
        }

        s.data = null;
            var beforeSend = s.beforeSend;
            s.beforeSend = function(xhr, o) {
                o.data = formdata;
                if(beforeSend)
                    beforeSend.call(this, xhr, o);
        };
        return $.ajax(s);
    }

    // private function for handling file uploads (hat tip to YAHOO!)
    function fileUploadIframe(a) {
        var form = $form[0], el, i, s, g, id, $io, io, xhr, sub, n, timedOut, timeoutHandle;
        var deferred = $.Deferred();

        if (a) {
            // ensure that every serialized input is still enabled
            for (i=0; i < elements.length; i++) {
                el = $(elements[i]);
                if ( hasProp )
                    el.prop('disabled', false);
                else
                    el.removeAttr('disabled');
            }
        }

        s = $.extend(true, {}, $.ajaxSettings, options);
        s.context = s.context || s;
        id = 'jqFormIO' + (new Date().getTime());
        if (s.iframeTarget) {
            $io = $(s.iframeTarget);
            n = $io.attr2('name');
            if (!n)
                 $io.attr2('name', id);
            else
                id = n;
        }
        else {
            $io = $('<iframe name="' + id + '" src="'+ s.iframeSrc +'" />');
            $io.css({ position: 'absolute', top: '-1000px', left: '-1000px' });
        }
        io = $io[0];


        xhr = { // mock object
            aborted: 0,
            responseText: null,
            responseXML: null,
            status: 0,
            statusText: 'n/a',
            getAllResponseHeaders: function() {},
            getResponseHeader: function() {},
            setRequestHeader: function() {},
            abort: function(status) {
                var e = (status === 'timeout' ? 'timeout' : 'aborted');
                log('aborting upload... ' + e);
                this.aborted = 1;

                try { // #214, #257
                    if (io.contentWindow.document.execCommand) {
                        io.contentWindow.document.execCommand('Stop');
                    }
                }
                catch(ignore) {}

                $io.attr('src', s.iframeSrc); // abort op in progress
                xhr.error = e;
                if (s.error)
                    s.error.call(s.context, xhr, e, status);
                if (g)
                    $.event.trigger("ajaxError", [xhr, s, e]);
                if (s.complete)
                    s.complete.call(s.context, xhr, e);
            }
        };

        g = s.global;
        // trigger ajax global events so that activity/block indicators work like normal
        if (g && 0 === $.active++) {
            $.event.trigger("ajaxStart");
        }
        if (g) {
            $.event.trigger("ajaxSend", [xhr, s]);
        }

        if (s.beforeSend && s.beforeSend.call(s.context, xhr, s) === false) {
            if (s.global) {
                $.active--;
            }
            deferred.reject();
            return deferred;
        }
        if (xhr.aborted) {
            deferred.reject();
            return deferred;
        }

        // add submitting element to data if we know it
        sub = form.clk;
        if (sub) {
            n = sub.name;
            if (n && !sub.disabled) {
                s.extraData = s.extraData || {};
                s.extraData[n] = sub.value;
                if (sub.type == "image") {
                    s.extraData[n+'.x'] = form.clk_x;
                    s.extraData[n+'.y'] = form.clk_y;
                }
            }
        }

        var CLIENT_TIMEOUT_ABORT = 1;
        var SERVER_ABORT = 2;
                
        function getDoc(frame) {
            /* it looks like contentWindow or contentDocument do not
             * carry the protocol property in ie8, when running under ssl
             * frame.document is the only valid response document, since
             * the protocol is know but not on the other two objects. strange?
             * "Same origin policy" http://en.wikipedia.org/wiki/Same_origin_policy
             */
            
            var doc = null;
            
            // IE8 cascading access check
            try {
                if (frame.contentWindow) {
                    doc = frame.contentWindow.document;
                }
            } catch(err) {
                // IE8 access denied under ssl & missing protocol
                log('cannot get iframe.contentWindow document: ' + err);
            }

            if (doc) { // successful getting content
                return doc;
            }

            try { // simply checking may throw in ie8 under ssl or mismatched protocol
                doc = frame.contentDocument ? frame.contentDocument : frame.document;
            } catch(err) {
                // last attempt
                log('cannot get iframe.contentDocument: ' + err);
                doc = frame.document;
            }
            return doc;
        }

        // Rails CSRF hack (thanks to Yvan Barthelemy)
        var csrf_token = $('meta[name=csrf-token]').attr('content');
        var csrf_param = $('meta[name=csrf-param]').attr('content');
        if (csrf_param && csrf_token) {
            s.extraData = s.extraData || {};
            s.extraData[csrf_param] = csrf_token;
        }

        // take a breath so that pending repaints get some cpu time before the upload starts
        function doSubmit() {
            // make sure form attrs are set
            var t = $form.attr2('target'), a = $form.attr2('action');

            // update form attrs in IE friendly way
            form.setAttribute('target',id);
            if (!method) {
                form.setAttribute('method', 'POST');
            }
            if (a != s.url) {
                form.setAttribute('action', s.url);
            }

            // ie borks in some cases when setting encoding
            if (! s.skipEncodingOverride && (!method || /post/i.test(method))) {
                $form.attr({
                    encoding: 'multipart/form-data',
                    enctype:  'multipart/form-data'
                });
            }

            // support timout
            if (s.timeout) {
                timeoutHandle = setTimeout(function() { timedOut = true; cb(CLIENT_TIMEOUT_ABORT); }, s.timeout);
            }

            // look for server aborts
            function checkState() {
                try {
                    var state = getDoc(io).readyState;
                    log('state = ' + state);
                    if (state && state.toLowerCase() == 'uninitialized')
                        setTimeout(checkState,50);
                }
                catch(e) {
                    log('Server abort: ' , e, ' (', e.name, ')');
                    cb(SERVER_ABORT);
                    if (timeoutHandle)
                        clearTimeout(timeoutHandle);
                    timeoutHandle = undefined;
                }
            }

            // add "extra" data to form if provided in options
            var extraInputs = [];
            try {
                if (s.extraData) {
                    for (var n in s.extraData) {
                        if (s.extraData.hasOwnProperty(n)) {
                           // if using the $.param format that allows for multiple values with the same name
                           if($.isPlainObject(s.extraData[n]) && s.extraData[n].hasOwnProperty('name') && s.extraData[n].hasOwnProperty('value')) {
                               extraInputs.push(
                               $('<input type="hidden" name="'+s.extraData[n].name+'">').val(s.extraData[n].value)
                                   .appendTo(form)[0]);
                           } else {
                               extraInputs.push(
                               $('<input type="hidden" name="'+n+'">').val(s.extraData[n])
                                   .appendTo(form)[0]);
                           }
                        }
                    }
                }

                if (!s.iframeTarget) {
                    // add iframe to doc and submit the form
                    $io.appendTo('body');
                    if (io.attachEvent)
                        io.attachEvent('onload', cb);
                    else
                        io.addEventListener('load', cb, false);
                }
                setTimeout(checkState,15);

                try {
                    form.submit();
                } catch(err) {
                    // just in case form has element with name/id of 'submit'
                    var submitFn = document.createElement('form').submit;
                    submitFn.apply(form);
                }
            }
            finally {
                // reset attrs and remove "extra" input elements
                form.setAttribute('action',a);
                if(t) {
                    form.setAttribute('target', t);
                } else {
                    $form.removeAttr('target');
                }
                $(extraInputs).remove();
            }
        }

        if (s.forceSync) {
            doSubmit();
        }
        else {
            setTimeout(doSubmit, 10); // this lets dom updates render
        }

        var data, doc, domCheckCount = 50, callbackProcessed;

        function cb(e) {
            if (xhr.aborted || callbackProcessed) {
                return;
            }
            
            doc = getDoc(io);
            if(!doc) {
                log('cannot access response document');
                e = SERVER_ABORT;
            }
            if (e === CLIENT_TIMEOUT_ABORT && xhr) {
                xhr.abort('timeout');
                deferred.reject(xhr, 'timeout');
                return;
            }
            else if (e == SERVER_ABORT && xhr) {
                xhr.abort('server abort');
                deferred.reject(xhr, 'error', 'server abort');
                return;
            }

            if (!doc || doc.location.href == s.iframeSrc) {
                // response not received yet
                if (!timedOut)
                    return;
            }
            if (io.detachEvent)
                io.detachEvent('onload', cb);
            else
                io.removeEventListener('load', cb, false);

            var status = 'success', errMsg;
            try {
                if (timedOut) {
                    throw 'timeout';
                }

                var isXml = s.dataType == 'xml' || doc.XMLDocument || $.isXMLDoc(doc);
                log('isXml='+isXml);
                if (!isXml && window.opera && (doc.body === null || !doc.body.innerHTML)) {
                    if (--domCheckCount) {
                        // in some browsers (Opera) the iframe DOM is not always traversable when
                        // the onload callback fires, so we loop a bit to accommodate
                        log('requeing onLoad callback, DOM not available');
                        setTimeout(cb, 250);
                        return;
                    }
                    // let this fall through because server response could be an empty document
                    //log('Could not access iframe DOM after mutiple tries.');
                    //throw 'DOMException: not available';
                }

                //log('response detected');
                var docRoot = doc.body ? doc.body : doc.documentElement;
                xhr.responseText = docRoot ? docRoot.innerHTML : null;
                xhr.responseXML = doc.XMLDocument ? doc.XMLDocument : doc;
                if (isXml)
                    s.dataType = 'xml';
                xhr.getResponseHeader = function(header){
                    var headers = {'content-type': s.dataType};
                    return headers[header];
                };
                // support for XHR 'status' & 'statusText' emulation :
                if (docRoot) {
                    xhr.status = Number( docRoot.getAttribute('status') ) || xhr.status;
                    xhr.statusText = docRoot.getAttribute('statusText') || xhr.statusText;
                }

                var dt = (s.dataType || '').toLowerCase();
                var scr = /(json|script|text)/.test(dt);
                if (scr || s.textarea) {
                    // see if user embedded response in textarea
                    var ta = doc.getElementsByTagName('textarea')[0];
                    if (ta) {
                        xhr.responseText = ta.value;
                        // support for XHR 'status' & 'statusText' emulation :
                        xhr.status = Number( ta.getAttribute('status') ) || xhr.status;
                        xhr.statusText = ta.getAttribute('statusText') || xhr.statusText;
                    }
                    else if (scr) {
                        // account for browsers injecting pre around json response
                        var pre = doc.getElementsByTagName('pre')[0];
                        var b = doc.getElementsByTagName('body')[0];
                        if (pre) {
                            xhr.responseText = pre.textContent ? pre.textContent : pre.innerText;
                        }
                        else if (b) {
                            xhr.responseText = b.textContent ? b.textContent : b.innerText;
                        }
                    }
                }
                else if (dt == 'xml' && !xhr.responseXML && xhr.responseText) {
                    xhr.responseXML = toXml(xhr.responseText);
                }

                try {
                    data = httpData(xhr, dt, s);
                }
                catch (err) {
                    status = 'parsererror';
                    xhr.error = errMsg = (err || status);
                }
            }
            catch (err) {
                log('error caught: ',err);
                status = 'error';
                xhr.error = errMsg = (err || status);
            }

            if (xhr.aborted) {
                log('upload aborted');
                status = null;
            }

            if (xhr.status) { // we've set xhr.status
                status = (xhr.status >= 200 && xhr.status < 300 || xhr.status === 304) ? 'success' : 'error';
            }

            // ordering of these callbacks/triggers is odd, but that's how $.ajax does it
            if (status === 'success') {
                if (s.success)
                    s.success.call(s.context, data, 'success', xhr);
                deferred.resolve(xhr.responseText, 'success', xhr);
                if (g)
                    $.event.trigger("ajaxSuccess", [xhr, s]);
            }
            else if (status) {
                if (errMsg === undefined)
                    errMsg = xhr.statusText;
                if (s.error)
                    s.error.call(s.context, xhr, status, errMsg);
                deferred.reject(xhr, 'error', errMsg);
                if (g)
                    $.event.trigger("ajaxError", [xhr, s, errMsg]);
            }

            if (g)
                $.event.trigger("ajaxComplete", [xhr, s]);

            if (g && ! --$.active) {
                $.event.trigger("ajaxStop");
            }

            if (s.complete)
                s.complete.call(s.context, xhr, status);

            callbackProcessed = true;
            if (s.timeout)
                clearTimeout(timeoutHandle);

            // clean up
            setTimeout(function() {
                if (!s.iframeTarget)
                    $io.remove();
                xhr.responseXML = null;
            }, 100);
        }

        var toXml = $.parseXML || function(s, doc) { // use parseXML if available (jQuery 1.5+)
            if (window.ActiveXObject) {
                doc = new ActiveXObject('Microsoft.XMLDOM');
                doc.async = 'false';
                doc.loadXML(s);
            }
            else {
                doc = (new DOMParser()).parseFromString(s, 'text/xml');
            }
            return (doc && doc.documentElement && doc.documentElement.nodeName != 'parsererror') ? doc : null;
        };
        var parseJSON = $.parseJSON || function(s) {
            /*jslint evil:true */
            return window['eval']('(' + s + ')');
        };

        var httpData = function( xhr, type, s ) { // mostly lifted from jq1.4.4

            var ct = xhr.getResponseHeader('content-type') || '',
                xml = type === 'xml' || !type && ct.indexOf('xml') >= 0,
                data = xml ? xhr.responseXML : xhr.responseText;

            if (xml && data.documentElement.nodeName === 'parsererror') {
                if ($.error)
                    $.error('parsererror');
            }
            if (s && s.dataFilter) {
                data = s.dataFilter(data, type);
            }
            if (typeof data === 'string') {
                if (type === 'json' || !type && ct.indexOf('json') >= 0) {
                    data = parseJSON(data);
                } else if (type === "script" || !type && ct.indexOf("javascript") >= 0) {
                    $.globalEval(data);
                }
            }
            return data;
        };

        return deferred;
    }
};

/**
 * ajaxForm() provides a mechanism for fully automating form submission.
 *
 * The advantages of using this method instead of ajaxSubmit() are:
 *
 * 1: This method will include coordinates for <input type="image" /> elements (if the element
 *    is used to submit the form).
 * 2. This method will include the submit element's name/value data (for the element that was
 *    used to submit the form).
 * 3. This method binds the submit() method to the form for you.
 *
 * The options argument for ajaxForm works exactly as it does for ajaxSubmit.  ajaxForm merely
 * passes the options argument along after properly binding events for submit elements and
 * the form itself.
 */
$.fn.ajaxForm = function(options) {
    options = options || {};
    options.delegation = options.delegation && $.isFunction($.fn.on);

    // in jQuery 1.3+ we can fix mistakes with the ready state
    if (!options.delegation && this.length === 0) {
        var o = { s: this.selector, c: this.context };
        if (!$.isReady && o.s) {
            log('DOM not ready, queuing ajaxForm');
            $(function() {
                $(o.s,o.c).ajaxForm(options);
            });
            return this;
        }
        // is your DOM ready?  http://docs.jquery.com/Tutorials:Introducing_$(document).ready()
        log('terminating; zero elements found by selector' + ($.isReady ? '' : ' (DOM not ready)'));
        return this;
    }

    if ( options.delegation ) {
        $(document)
            .off('submit.form-plugin', this.selector, doAjaxSubmit)
            .off('click.form-plugin', this.selector, captureSubmittingElement)
            .on('submit.form-plugin', this.selector, options, doAjaxSubmit)
            .on('click.form-plugin', this.selector, options, captureSubmittingElement);
        return this;
    }

    return this.ajaxFormUnbind()
        .bind('submit.form-plugin', options, doAjaxSubmit)
        .bind('click.form-plugin', options, captureSubmittingElement);
};

// private event handlers
function doAjaxSubmit(e) {
    /*jshint validthis:true */
    var options = e.data;
    if (!e.isDefaultPrevented()) { // if event has been canceled, don't proceed
        e.preventDefault();
        $(this).ajaxSubmit(options);
    }
}

function captureSubmittingElement(e) {
    /*jshint validthis:true */
    var target = e.target;
    var $el = $(target);
    if (!($el.is("[type=submit],[type=image]"))) {
        // is this a child element of the submit el?  (ex: a span within a button)
        var t = $el.closest('[type=submit]');
        if (t.length === 0) {
            return;
        }
        target = t[0];
    }
    var form = this;
    form.clk = target;
    if (target.type == 'image') {
        if (e.offsetX !== undefined) {
            form.clk_x = e.offsetX;
            form.clk_y = e.offsetY;
        } else if (typeof $.fn.offset == 'function') {
            var offset = $el.offset();
            form.clk_x = e.pageX - offset.left;
            form.clk_y = e.pageY - offset.top;
        } else {
            form.clk_x = e.pageX - target.offsetLeft;
            form.clk_y = e.pageY - target.offsetTop;
        }
    }
    // clear form vars
    setTimeout(function() { form.clk = form.clk_x = form.clk_y = null; }, 100);
}


// ajaxFormUnbind unbinds the event handlers that were bound by ajaxForm
$.fn.ajaxFormUnbind = function() {
    return this.unbind('submit.form-plugin click.form-plugin');
};

/**
 * formToArray() gathers form element data into an array of objects that can
 * be passed to any of the following ajax functions: $.get, $.post, or load.
 * Each object in the array has both a 'name' and 'value' property.  An example of
 * an array for a simple login form might be:
 *
 * [ { name: 'username', value: 'jresig' }, { name: 'password', value: 'secret' } ]
 *
 * It is this array that is passed to pre-submit callback functions provided to the
 * ajaxSubmit() and ajaxForm() methods.
 */
$.fn.formToArray = function(semantic, elements) {
    var a = [];
    if (this.length === 0) {
        return a;
    }

    var form = this[0];
    var els = semantic ? form.getElementsByTagName('*') : form.elements;
    if (!els) {
        return a;
    }

    var i,j,n,v,el,max,jmax;
    for(i=0, max=els.length; i < max; i++) {
        el = els[i];
        n = el.name;
        if (!n || el.disabled) {
            continue;
        }

        if (semantic && form.clk && el.type == "image") {
            // handle image inputs on the fly when semantic == true
            if(form.clk == el) {
                a.push({name: n, value: $(el).val(), type: el.type });
                a.push({name: n+'.x', value: form.clk_x}, {name: n+'.y', value: form.clk_y});
            }
            continue;
        }

        v = $.fieldValue(el, true);
        if (v && v.constructor == Array) {
            if (elements)
                elements.push(el);
            for(j=0, jmax=v.length; j < jmax; j++) {
                a.push({name: n, value: v[j]});
            }
        }
        else if (feature.fileapi && el.type == 'file') {
            if (elements)
                elements.push(el);
            var files = el.files;
            if (files.length) {
                for (j=0; j < files.length; j++) {
                    a.push({name: n, value: files[j], type: el.type});
                }
            }
            else {
                // #180
                a.push({ name: n, value: '', type: el.type });
            }
        }
        else if (v !== null && typeof v != 'undefined') {
            if (elements)
                elements.push(el);
            a.push({name: n, value: v, type: el.type, required: el.required});
        }
    }

    if (!semantic && form.clk) {
        // input type=='image' are not found in elements array! handle it here
        var $input = $(form.clk), input = $input[0];
        n = input.name;
        if (n && !input.disabled && input.type == 'image') {
            a.push({name: n, value: $input.val()});
            a.push({name: n+'.x', value: form.clk_x}, {name: n+'.y', value: form.clk_y});
        }
    }
    return a;
};

/**
 * Serializes form data into a 'submittable' string. This method will return a string
 * in the format: name1=value1&amp;name2=value2
 */
$.fn.formSerialize = function(semantic) {
    //hand off to jQuery.param for proper encoding
    return $.param(this.formToArray(semantic));
};

/**
 * Serializes all field elements in the jQuery object into a query string.
 * This method will return a string in the format: name1=value1&amp;name2=value2
 */
$.fn.fieldSerialize = function(successful) {
    var a = [];
    this.each(function() {
        var n = this.name;
        if (!n) {
            return;
        }
        var v = $.fieldValue(this, successful);
        if (v && v.constructor == Array) {
            for (var i=0,max=v.length; i < max; i++) {
                a.push({name: n, value: v[i]});
            }
        }
        else if (v !== null && typeof v != 'undefined') {
            a.push({name: this.name, value: v});
        }
    });
    //hand off to jQuery.param for proper encoding
    return $.param(a);
};

/**
 * Returns the value(s) of the element in the matched set.  For example, consider the following form:
 *
 *  <form><fieldset>
 *      <input name="A" type="text" />
 *      <input name="A" type="text" />
 *      <input name="B" type="checkbox" value="B1" />
 *      <input name="B" type="checkbox" value="B2"/>
 *      <input name="C" type="radio" value="C1" />
 *      <input name="C" type="radio" value="C2" />
 *  </fieldset></form>
 *
 *  var v = $('input[type=text]').fieldValue();
 *  // if no values are entered into the text inputs
 *  v == ['','']
 *  // if values entered into the text inputs are 'foo' and 'bar'
 *  v == ['foo','bar']
 *
 *  var v = $('input[type=checkbox]').fieldValue();
 *  // if neither checkbox is checked
 *  v === undefined
 *  // if both checkboxes are checked
 *  v == ['B1', 'B2']
 *
 *  var v = $('input[type=radio]').fieldValue();
 *  // if neither radio is checked
 *  v === undefined
 *  // if first radio is checked
 *  v == ['C1']
 *
 * The successful argument controls whether or not the field element must be 'successful'
 * (per http://www.w3.org/TR/html4/interact/forms.html#successful-controls).
 * The default value of the successful argument is true.  If this value is false the value(s)
 * for each element is returned.
 *
 * Note: This method *always* returns an array.  If no valid value can be determined the
 *    array will be empty, otherwise it will contain one or more values.
 */
$.fn.fieldValue = function(successful) {
    for (var val=[], i=0, max=this.length; i < max; i++) {
        var el = this[i];
        var v = $.fieldValue(el, successful);
        if (v === null || typeof v == 'undefined' || (v.constructor == Array && !v.length)) {
            continue;
        }
        if (v.constructor == Array)
            $.merge(val, v);
        else
            val.push(v);
    }
    return val;
};

/**
 * Returns the value of the field element.
 */
$.fieldValue = function(el, successful) {
    var n = el.name, t = el.type, tag = el.tagName.toLowerCase();
    if (successful === undefined) {
        successful = true;
    }

    if (successful && (!n || el.disabled || t == 'reset' || t == 'button' ||
        (t == 'checkbox' || t == 'radio') && !el.checked ||
        (t == 'submit' || t == 'image') && el.form && el.form.clk != el ||
        tag == 'select' && el.selectedIndex == -1)) {
            return null;
    }

    if (tag == 'select') {
        var index = el.selectedIndex;
        if (index < 0) {
            return null;
        }
        var a = [], ops = el.options;
        var one = (t == 'select-one');
        var max = (one ? index+1 : ops.length);
        for(var i=(one ? index : 0); i < max; i++) {
            var op = ops[i];
            if (op.selected) {
                var v = op.value;
                if (!v) { // extra pain for IE...
                    v = (op.attributes && op.attributes['value'] && !(op.attributes['value'].specified)) ? op.text : op.value;
                }
                if (one) {
                    return v;
                }
                a.push(v);
            }
        }
        return a;
    }
    return $(el).val();
};

/**
 * Clears the form data.  Takes the following actions on the form's input fields:
 *  - input text fields will have their 'value' property set to the empty string
 *  - select elements will have their 'selectedIndex' property set to -1
 *  - checkbox and radio inputs will have their 'checked' property set to false
 *  - inputs of type submit, button, reset, and hidden will *not* be effected
 *  - button elements will *not* be effected
 */
$.fn.clearForm = function(includeHidden) {
    return this.each(function() {
        $('input,select,textarea', this).clearFields(includeHidden);
    });
};

/**
 * Clears the selected form elements.
 */
$.fn.clearFields = $.fn.clearInputs = function(includeHidden) {
    var re = /^(?:color|date|datetime|email|month|number|password|range|search|tel|text|time|url|week)$/i; // 'hidden' is not in this list
    return this.each(function() {
        var t = this.type, tag = this.tagName.toLowerCase();
        if (re.test(t) || tag == 'textarea') {
            this.value = '';
        }
        else if (t == 'checkbox' || t == 'radio') {
            this.checked = false;
        }
        else if (tag == 'select') {
            this.selectedIndex = -1;
        }
		else if (t == "file") {
			if (/MSIE/.test(navigator.userAgent)) {
				$(this).replaceWith($(this).clone(true));
			} else {
				$(this).val('');
			}
		}
        else if (includeHidden) {
            // includeHidden can be the value true, or it can be a selector string
            // indicating a special test; for example:
            //  $('#myForm').clearForm('.special:hidden')
            // the above would clean hidden inputs that have the class of 'special'
            if ( (includeHidden === true && /hidden/.test(t)) ||
                 (typeof includeHidden == 'string' && $(this).is(includeHidden)) )
                this.value = '';
        }
    });
};

/**
 * Resets the form data.  Causes all form elements to be reset to their original value.
 */
$.fn.resetForm = function() {
    return this.each(function() {
        // guard against an input with the name of 'reset'
        // note that IE reports the reset function as an 'object'
        if (typeof this.reset == 'function' || (typeof this.reset == 'object' && !this.reset.nodeType)) {
            this.reset();
        }
    });
};

/**
 * Enables or disables any matching elements.
 */
$.fn.enable = function(b) {
    if (b === undefined) {
        b = true;
    }
    return this.each(function() {
        this.disabled = !b;
    });
};

/**
 * Checks/unchecks any matching checkboxes or radio buttons and
 * selects/deselects and matching option elements.
 */
$.fn.selected = function(select) {
    if (select === undefined) {
        select = true;
    }
    return this.each(function() {
        var t = this.type;
        if (t == 'checkbox' || t == 'radio') {
            this.checked = select;
        }
        else if (this.tagName.toLowerCase() == 'option') {
            var $sel = $(this).parent('select');
            if (select && $sel[0] && $sel[0].type == 'select-one') {
                // deselect all other options
                $sel.find('option').selected(false);
            }
            this.selected = select;
        }
    });
};

// expose debug var
$.fn.ajaxSubmit.debug = false;

// helper fn for console logging
function log() {
    if (!$.fn.ajaxSubmit.debug)
        return;
    var msg = '[jquery.form] ' + Array.prototype.join.call(arguments,'');
    if (window.console && window.console.log) {
        window.console.log(msg);
    }
    else if (window.opera && window.opera.postError) {
        window.opera.postError(msg);
    }
}

})(jQuery);

/**
 * 以下内容为扩展内容,为ajax自动加载数据请求
 * The following is an extension of the content for the Ajax to automatically load the data request.
 */
;(function($) {
	$.fn.formStartPaging = function(url,callback){
		jQuery.ajaxDeliy();
		var thisObject = this;
		$.ajax({url:url,success:function(thisContext){
			if(callback!=undefined && typeof(callback)=='function'){callback(thisContext);}else{$(thisObject).html(thisContext);};	
			jQuery.closeDeliy();
			$(thisObject).find("#Paging").find("a").click(function(){ 
				var href = $(this).attr("href");
				if(href.indexOf('javascript')==-1){
					var href = $(this).attr("href");
					$(thisObject).formStartPaging(href,callback);
					return false;
				}
				else{return false;	}							   
			});
			$(thisObject).find("#frm-select-chg").change(function(){
				var href = $(this).val();
				$(thisObject).formStartPaging(href,callback);
				return false;
			});
			
		}});	
	};
	$.fn.post=function(back){
		if(this[0]!=undefined && _doPost(this[0])){
			try{
				if(!document.getElementById("frm-dealy-submit-box")){
					$(document.body).append("<div id=\"frm-dealy-submit-box\"></div>");	
				}	
			}catch(err){}
			try{
				if(!document.getElementById("frm-asyn-hidden")){$(this[0]).append("<input type=\"hidden\" id=\"frm-asyn-hidden\" name=\"isAsyn\" value=\"1\" />");}	
			}catch(err){}
			$(this[0]).ajaxSubmit({success:function(responseText){
				try{
					if(back!=undefined && typeof(back)=='function'){
						back($.parseJSON(responseText));
					}	
				}catch(err){}
				/*移除提示信息*/
				try{
					if(document.getElementById("frm-dealy-submit-box")){
						//$(document.getElementById("frm-dealy-submit-box")).remove();	
					}
				}catch(err){}
				
			}});
		}
		else{return false;};
		return false;
	}
	
	
})(jQuery);

$(function(){
	/*******************************************************************
	*验证日期格式
	********************************************************************/
	$("input[isnumeric='true']").keyup(function(){
		if(isNaN(this.value)){this.value=0;}	
	});
	/*******************************************************************
	*验证为空
	********************************************************************/
	$("input[notkong='true']").blur(function(){
		try{
			if(this.value!=undefined && this.value!="")
			{
				$(this).attr("complate","false");
			}else{
				$(this).attr("complate","true");	
			}
		}catch(err){}
	});
	/*******************************************************************
	*全选
	********************************************************************/
	$("input[operate='selectList']").click(function(){
		var form=this.form;
		for(var n=0;n<form.elements.length;n++){
			if(form.elements[n].type=='checkbox'){
				form.elements[n].checked=this.checked;
			}
		}
	});
	/*******************************************************************
	*提交数据信息
	********************************************************************/
	$("#frm-submit-btns").click(function(){
		try{
			var ajax = $(this).attr("ajax") || "true";
			if(ajax!=undefined && ajax!="false")
			{
				if(doPost(this))
				{ajaxSubmit(this.form);}
				else{return false;};
			}
			else{
				if(doPost(this))
				{this.form.submit();}
				else{return false;};
			}
			return false;	
		}catch(err){}
	});
	/*******************************************************************
	*提交数据
	********************************************************************/
	$("input[operate=\"submit\"]").click(function(){
		try{
			var ajax = $(this).attr("ajax") || "true";
			if(ajax!=undefined && ajax!="false")
			{
				if(doPost(this))
				{ajaxSubmit(this.form);}
				else{return false;};
			}
			else{
				if(doPost(this))
				{this.form.submit();}
				else{return false;};
			}
			return false;	
		}catch(err){}
	});
	/*******************************************************************
	*更新数据排序
	********************************************************************/
	$('input[operate=\"editor\"]').change(function(){	
		try{
			var $this = this;
			var $url = $($this).attr("url") || "";
			/*******************************************************************
			*重新解析请求url
			********************************************************************/
			if($url!=undefined && $url!=""){
				$url=$url+'&isAsyn=1';
				var thisValue = $($this).val() || "";
				if(thisValue!=undefined && thisValue!=""){
					$url = $url + '&val=' + thisValue;	
				}
			}
			/*******************************************************************
			*重新解析请求url
			********************************************************************/
			if($url!=undefined && $url!="")
			{
				jQuery.ajax({url:$url,type:"get",
					  success:function(strResponse)
					  {
						  if(strResponse!=undefined && strResponse!='' 
						  && strResponse.indexOf('{')!=-1 && strResponse.indexOf('}')!=-1)
						  {
							  var options = {} ;
							  try{ var options = jQuery.parseJSON(''+strResponse+'') || {};}
							  catch(err){}
							  try{
								options['type'] = options['type'] || 'alert';
								options['tips'] = options['tips'] || '';
							  }catch(err){}
							  try{SubmitHandleResponse(options);}catch(err){}
						  }
						  else if(strResponse!=undefined && strResponse!=""){
							  try{
								messagerAlert({
									'tips':strResponse,
									'type':'alert',
									'success':'false',
									'interval':10
								});
							  }catch(err){}  	  
						  }
					  },
					  error:function(){
							try{
								messagerAlert({
									'tips':'请求过程中发生错误,或返回格式错误无法解析',
									'type':'alert',
									'success':'false',
									'interval':10
								});
							}catch(err){}  
					  }
				});
			}	
		}catch(err){}
	});
});
/*********************************************************************************
*form post submit
**********************************************************************************/
var _doPost=function($this){
	
	if($this.form==undefined){return false;}
	/*****************************************************************************
	* 开始判断验证数据表单
	******************************************************************************/
	var isTrue = true;
	for(var k=0;k<$this.form.elements.length;k++){
		try{
			var Element = $this.form.elements[k];
			
			if($(Element).attr("notkong")=="true" && $(Element).val()=="")
			{$(Element).attr("complate","true");$(Element).focus();isTrue=false;break;}
			else{$(Element).removeAttr("complate");}
			
			if($(Element).attr("notkong")=="true" && $(Element).val()==$(Element).attr("char"))
			{$(Element).attr("complate","true");;isTrue=false;break;}
			else{$(Element).removeAttr("complate");}
			
			if($(Element).attr("isnumeric")=="true" && isNaN($(Element).val()))
			{$(Element).attr("complate","true");isTrue=false;break;}
			else{$(Element).removeAttr("complate");}
			
			if($(Element).attr("isdate")=="true" && !CheckDateTime($(Element).val()))
			{$(Element).attr("complate","true");isTrue=false;break;}
			else{$(Element).removeAttr("complate");}
			
		}catch(err){}
	}
	/*****************************************************************************
	* 返回数据验证结果
	******************************************************************************/
	return isTrue;
}
/*********************************************************************************
*btns post submit
**********************************************************************************/
var doPost=function($this){
	
	if($this.form==undefined){return false;}
	/*****************************************************************************
	* 开始判断验证数据表单
	******************************************************************************/
	var isTrue = true;
	for(var k=0;k<$this.form.elements.length;k++){
		try{
			var Element = $this.form.elements[k];
			
			if($(Element).attr("notkong")=="true" && $(Element).val()=="")
			{$(Element).attr("complate","true");$(Element).focus();isTrue=false;break;}
			else{$(Element).removeAttr("complate");}
			
			if($(Element).attr("notkong")=="true" && $(Element).val()==$(Element).attr("char"))
			{$(Element).attr("complate","true");;isTrue=false;break;}
			else{$(Element).removeAttr("complate");}
			
			if($(Element).attr("isnumeric")=="true" && isNaN($(Element).val()))
			{$(Element).attr("complate","true");isTrue=false;break;}
			else{$(Element).removeAttr("complate");}
			
			if($(Element).attr("isdate")=="true" && !CheckDateTime($(Element).val()))
			{$(Element).attr("complate","true");isTrue=false;break;}
			else{$(Element).removeAttr("complate");}
			
		}catch(err){}
	}
	/*****************************************************************************
	* 返回数据验证结果
	******************************************************************************/
	return isTrue;
}
/*************************************************************************
*获取编辑器内容
**************************************************************************/
var GetEditorContent = function(EditorName) {
	try{
		var oEditor = FCKeditorAPI.GetInstance(EditorName);
		try{
			$("#"+EditorName).val(oEditor.EditorDocument.body.innerHTML);
		}catch(err){
			if(document.getElementById(EditorName))	{
				document.getElementById(EditorName).value =	oEditor.EditorDocument.body.innerHTML;
			}
		}
	}catch(err){}
}
/*************************************************************************
*开始异步提交数据
**************************************************************************/
var ajaxSubmit =function($this,callback){
	/*********************************************************************
	*使用异步提交的方式返回数据信息
	**********************************************************************/
	var $this = $this || null;
	try{
		if($this!=undefined && $this!=null && !$this["isAsyn"])
		{
			var strAsyn="<input type=\"hidden\" name=\"isAsyn\" value=\"1\" />";
			$($this).append(strAsyn);	
		}
	}catch(err){}
	/*********************************************************************
	*插入提示信息
	**********************************************************************/
	try{
		if(!document.getElementById("frm-dealy-submit-box"))
		{
			$(document.body).append("<div id=\"frm-dealy-submit-box\"></div>");	
		}
	}catch(err){}
	/*********************************************************************
	*装载编辑器中的内容
	**********************************************************************/
	try{
		$("iframe[operate=\"editor\"]").each(function(){
			var editorName = $(this).attr("instance");
			if(editorName==undefined || editorName==""){editorName = "Content";	}
			GetEditorContent(editorName);
		});
	}catch(err){}
	/*****************************************************************************
	* 开始提交数据
	******************************************************************************/
	try{
		$($this).ajaxSubmit({
			dataType:"json",
			success:function(strResponse){
				/*********************************************************************
				*移除提示信息
				**********************************************************************/
				try{
					if(document.getElementById("frm-dealy-submit-box")){
						$(document.getElementById("frm-dealy-submit-box")).remove();	
					}
				}catch(err){}
				/*********************************************************************
				*开始展示处理结果
				**********************************************************************/
				if(strResponse!=undefined &&  strResponse!='' 
				&& callback!=undefined && typeof(callback)=='function')
				{
					try{callback(strResponse);}
					catch(err){}
				}
				else if(strResponse!=undefined && typeof(strResponse)=='object')
				{
					/*********************************************************************
					*转换Json格式
					**********************************************************************/
					var options = {};
					try{options = eval(strResponse) || {};}catch(err){alert(err.message);}
					/*********************************************************************
					*判断本地是否存在回调对象,否则执行操作
					**********************************************************************/
					try{
						if(options!=undefined && typeof(options)=='object' 
						&& window.SaveBack!=undefined && window.SaveBack!=null && typeof(window.SaveBack)=='function' 
						&& options['success']!=undefined && options['success']=='true')
						{
							try{window.SaveBack(options,function(){
								try{SubmitHandleResponse(options);}
								catch(err){}							  
							});}
							catch(err){}
						}else{
							try{SubmitHandleResponse(options);}
							catch(err){}		
						}
					}catch(err){}
				}
				/*********************************************************************
				*移除确认控件锁定
				**********************************************************************/
				try{
					$($this).find("input[type=\"submit\"]").removeAttr("disabled");	
				}catch(err){}
			},
			error:function(message){
				/*********************************************************************
				*移除提示信息
				**********************************************************************/
				try{
					if(document.getElementById("frm-dealy-submit-box")){
						$(document.getElementById("frm-dealy-submit-box")).remove();	
					}
				}catch(err){}
				/*********************************************************************
				*处理数据结果信息
				**********************************************************************/
				try{
					messagerAlert({
						'tips':'请求过程中发生错误,或返回格式错误无法解析',
						'type':'alert',
						'success':'false',
						'interval':10
					});
				}catch(err){}
			}
		});
		return false;
	}catch(err){$.messager.alert("系统提示",err.message,"error");}
}
/****************************************************************************************
*处理服务器相应的数据信息
*****************************************************************************************/
var SubmitHandleResponse=function(options)
{
	/*********************************************************************
	*格式化请求数据模式
	***********************************************************************/
	try{
		var options = options || {};
		try{options['type']=options['type'] || 'alert';	}catch(err){};
		try{options['success']=options['success'] || 'false';}catch(err){};
		try{options['tips'] = options['tips'] || '来自客户端的反馈发生未知错误,请重试!';}catch(err){};
	}catch(err){}

	/*********************************************************************
	*开始处理网站数据信息
	***********************************************************************/
	if(options!=undefined && typeof(options)=='object' 
	&& options['type']!=undefined && options['type']=='confirm')
	{	
		try{messagerConfirm(options);}catch(err){};
	}
	else if(options!=undefined && typeof(options)=='object' 
	&& options['type']!=undefined && options['type']=='alert')
	{
		try{messagerAlert(options);}catch(err){};
	}
	else if(options!=undefined && typeof(options)=='object' 
	&& options['type']!=undefined && options['type']=='redirect'){
		try{
			if(options['url']!=undefined && options['url']!=''){
				window.location=options['url'];	
			}else{
				window.location='?action=default';	
			}
		}catch(err){}
	}else if(options!=undefined && typeof(options)=='object'
	&& options['url']!=undefined && options['url']!='')
	{
		window.location	= options['url'] ;	 
	}else{
		try{
			messagerAlert({'tips':'请求过程中无响应结果,请重试','type':'alert','interval':'10'});
		}catch(err){};	
	}
}
/****************************************************************************************
*弹出确认提示框数据信息
*****************************************************************************************/
var messagerConfirm = function(options)
{
	/****************************************************************************************
	*初始化数据信息
	*****************************************************************************************/
	try{
		var options = options || {};
		options['falseUrl'] = options['falseUrl'] || '?action=default';
		options['tips'] = options['tips'] || '数据处理成功,点击确定将继续停留在当前界面';
	}catch(err){}
	/****************************************************************************************
	*开始加载确认模块
	*****************************************************************************************/
	try{
		if($.messager!=undefined && typeof($.messager)=='object' 
		&& $.messager.confirm!=undefined && typeof($.messager.confirm)=='function')
		{
			$.messager.confirm('系统提示',options["tips"],function(confirmOK){
				if(confirmOK){
					if(options['trueUrl']!=undefined && options['trueUrl']!=''){
						window.location=jsn["trueUrl"];	
					}	
				}else{
					if(options["falseUrl"]!=undefined && options["falseUrl"]!="")
					{window.location=options["falseUrl"];}
					else{window.location="?action=default";}
				}
			});
		}else{
			if(confirm(options["tips"])){
				if(options['trueUrl']!=undefined && options['trueUrl']!=''){
					window.location = options['trueUrl'];	
				}
			}else{
				if(options['falseUrl']!=undefined && options['falseUrl']!=''){
					window.location = options['falseUrl'];	
				}else{
					window.location="?action=default";
				}
			}
		}
	}catch(err){}	
}
/****************************************************************************************
*弹出数据提示框信息
*****************************************************************************************/
var messagerAlert = function(options)
{
	/****************************************************************************************
	*初始化数据信息
	*****************************************************************************************/
	try{
		var options = options || {};
		options['interval'] = parseInt(options['interval']) || 10;
		options['tips'] = options['tips'] || '';
	}catch(err){}
	/****************************************************************************************
	*开始处理数据信息
	*****************************************************************************************/
	if(options!=undefined && typeof(options)=='object' 
	&& options['interval']!=undefined && options['interval']!=0)
	{
		try{
			if(document.getElementById('frm-messager-alert')!=undefined
			&& document.getElementById('frm-messager-alert')!=null){
				$(document.getElementById('frm-messager-alert')).remove();	
			}
		}catch(err){}
		/****************************************************************************************
		*准备创建数据节点
		*****************************************************************************************/
		var $this = document.createElement('div');
		try{
			var $this = document.createElement('div');
			$this.id='frm-messager-alert';
		}catch(err){}
		/****************************************************************************************
		*设置操作结果属性信息
		*****************************************************************************************/
		try{
			if(options['success']=='true'){
				$($this).attr('success','true');	
			}else{
				$($this).attr('success','false');
			}	
		}catch(err){}
		/************************************************************************
		*将数据节点追加到body
		*************************************************************************/
		try{
			if(document.body!=undefined && document.body!=null 
			&& $this!=undefined && $this!=null)
			{$(document.body).append($this);}
		}catch(err){}
		/************************************************************************
		*设置显示内容信息
		*************************************************************************/
		try{
			var $set = setTimeout(function(){
				$($this).html(options['tips']);	
				clearTimeout($set);
			},200);
		}catch(err){}
		/************************************************************************
		*将数据节点追加到body
		*************************************************************************/
		try{
			var $timer = parseInt(options['interval']) || 10;
			var $obj = setInterval(function(){
				if($timer>=1){
					$timer=$timer-1;
				}
				else{
					/************************************************************
					*清除定时器
					*************************************************************/
					clearInterval($obj);
					/************************************************************
					*溢出当前插件
					*************************************************************/
					try{$($this).remove();}catch(err){};
					/************************************************************
					*执行数据回调
					*************************************************************/
					try{
						if(options['back']!=undefined 
						&& typeof(options['back'])=='function'){
							options['back']();	
						}
						else if(options['url']!=undefined 
						&& options['url']!='')
						{
							window.location=options['url'];	
						}	
					}catch(err){};
					
				}
			},1000);
			
		}catch(err){}
	}
	else if($.messager!=undefined && typeof($.messager)=='object' 
	&& $.messager.alert!=undefined && typeof($.messager.alert)=='function')
	{
		try{
			var showIcons = 'error';
			if(options['success']!=undefined && options['success']=='true'){
				showIcons = 'info';	
			}
		}catch(err){}
		try{
			$.messager.alert('系统提示',options['tips'],showIcons,function(){
				try{
					if(options['back']!=undefined 
					&& typeof(options['back'])=='function')
					{
						options['back']();	
					}else if(options['url']!=undefined 
					&& options['url']!="")
					{
						window.location=options['url'];	
					}	
				}catch(err){}
			});
		}catch(err){}
	}else{
		try{
			window.alert(options['tips']);
			return false;
		}catch(err){}
	}
}

/****************************************************************************************
*验证日期格式是否合法
*****************************************************************************************/
function CheckDateTime(str){
	try{
		var reg = /^(\d+)-(\d{1,2})-(\d{1,2}) (\d{1,2}):(\d{1,2}):(\d{1,2})$/;
		var r = str.match(reg);
		if(r==null)return false;
		r[2]=r[2]-1;
		var d= new Date(r[1], r[2],r[3], r[4],r[5], r[6]);
		if(d.getFullYear()!=r[1])return false;
		if(d.getMonth()!=r[2])return false;
		if(d.getDate()!=r[3])return false;
		if(d.getHours()!=r[4])return false;
		if(d.getMinutes()!=r[5])return false;
		if(d.getSeconds()!=r[6])return false;
		return true;
	}catch(err){return false;}
}
/*************************************************************************************
*ajax加载网页数据
**************************************************************************************/
;(function($){
	/****************************************************************************
	*加载网页内容
	*****************************************************************************/
	$.fn.pager = function(url,callback)
	{
		try{
			var thisNode = document.createElement('div');
			thisNode.id='frm-dealy-submit-box';
			if(document.getElementById(thisNode.id)==null){
				$(document.body).append(thisNode);	
			}	
		}catch(err){}
		var thisObject = this;
		$.ajax({url:url ,type:"get",async: true,
			success:function(strxml){
				if(callback!=undefined){callback(strxml);}
				else{$(thisObject).html(strxml);}
				$(thisObject).find("#fenyePageBar").find("a").click(function(){ 
					var href = $(this).attr("href");
					if(href.indexOf('javascript')==-1){
						var href = $(this).attr("href");
						$(thisObject).pager(href,callback);
						return false;
					}
					else{return false;	}							   
				});
				$(thisObject).find("#frm-select-chg").change(function(){
					var href = $(this).val();
					$(thisObject).pager(href,callback);
					return false;
				});
				/*************************************************************
				*移除加载控件
				**************************************************************/
				try{
					if(document.getElementById(thisNode.id)){
						$(thisNode).remove();	
					}	
				}catch(err){}
			}
		});
	};
	/****************************************************************************
	*加载倒计时
	*****************************************************************************/
	$.fn.interval=function(back){
		var interval = parseInt($(this).attr("value")) || 0;
		var thisT = this;
		if(interval<=0){
			if(back!=undefined && typeof(back)=='function'){
				back(0);
			};
			$(thisT).html("<font>00</font> <font>00</font>:<font>00</font>:<font>00</font>");	
		}
		else{
			try{
				var object = setInterval(function(){
					if(interval>0){
						interval=interval-1;
						var days = parseInt(interval / 86400);
						if(days<10){days="0"+days;}
						var hour = parseInt((interval % 86400) / 3600);
						if(hour<10){hour="0"+hour;}
						var Minute = parseInt((interval % 3600) / 60);
						if(Minute<10){Minute="0"+Minute;}
						var Second = parseInt(((interval % 3600) % 60 % 60));
						if(Second<10){Second="0"+Second;}
						$(thisT).html("<font>"+days+"</font> <font>"+hour + "</font>:<font>" + Minute + "</font>:<font>"+Second +"</font>");
					}
					else{
						clearInterval(object);
						if(back!=undefined && typeof(back)=='function'){
							back(0);
						};
						$(thisT).html("<font>00</font> <font>00</font>:<font>00</font>:<font>00</font>");
					}
				},1000);
			}catch(err){}
		}
	};
	/****************************************************************************
	*复制网页内容信息
	*****************************************************************************/
	$.fn.copy=function(text,back){
		try{
			
			var clip = new ZeroClipboard.Client(); // 新建一个对象
			clip.setHandCursor(true);
			clip.setText(text); // 设置要复制的文本。
			clip.addEventListener("mouseup", function(client) {
				$(document.body).append("<span id=\"frm-copy-success\">Copy Success</span>");
				var timer = 3;
				var obj=setInterval(function(){
					if(timer>=0){timer=timer-1;}
					else{clearInterval(obj);$("#frm-copy-success").remove();timer = 3;}
				}
				,100)
			});
			// 注册一个 button，参数为 id。点击这个 button 就会复制。
			//这个 button 不一定要求是一个 input 按钮，也可以是其他 DOM 元素。
			if(this!=undefined && this[0]!=undefined && this[0]!=null)
			{clip.glue(this[0]);}
		}catch(err){alert(err.message);}
	}
	/****************************************************************************
	*展示数据提示信息
	*****************************************************************************/
	$.fn.tips=function(options){
		
		try{
			var options = options || {tips:'',success:false};
		}
		catch(err){}
		/**************************************************************
		*获取处理节点
		***************************************************************/
		var $this = null;
		try{
			$this= this[0] || document.getElementById('frm-alert');	
		}catch(err){}
		/**************************************************************
		*处理节点不存在,执行数据创建
		***************************************************************/
		try{
			if(($this==undefined || $this==null)){
				$this = document.createElement('div');
				$this.id='frm-alert';
				if(document.body!=undefined && document.body!=null){
					$(document.body).append($this);	
					iCreated = true;
				}
			};
		}catch(err){}
		
		if($this!=null && $this!=undefined 
		&& options!=undefined && typeof(options)=='object')
		{
			try{
				if(options['success']==undefined || options['success']==''){
					options['success']=false;	
				}
				if(options['success']!=undefined && options['success']!="")
				{
					$($this).attr('success',options['success']);	
				}
			}catch(err){}
			try{
				if(options['tips']!=undefined && options['tips']!=""){
					$($this).html(options['tips']);	
				}
			}catch(err){}
			
			try{
				$($this).show('slow');
			}catch(err){}
			/**************************************************************
			*设置倒计时
			***************************************************************/
			try{
				var timer = parseInt(options['timer']) || 10;
				var $object = setInterval(function(){
					if(timer>=1){timer=timer-1;}
					else{
						clearInterval($object);
						try{
							if($this!=undefined && $this!=null){
								$($this).hide('slow');
							}
						}catch(err){}
						try{
							if(options['back']!=undefined && typeof(options['back'])=='function'){
								options['back']();	
							}
						}catch(err){}
					}
				},1000);	
			}catch(err){}
		}
		
	};
	$.fn.contianer = function(options)
	{
		if(options!=undefined && options!=null 
		&& typeof(options)=='string' && options=='close')
		{
			$(this).hide('slow');
		}
		else
		{
			var $this = this;
			try{
				var options = options || {};
				if(options['name']==undefined || options['name']=='')
				{options['name'] = 'define';}	
			}catch(err){}
			/*****************************************************************
			*显示延时加载的效果信息
			******************************************************************/
			try{
				$($this).attr('status','load');
				$($this).show('slow');
			}catch(err){}
			/*****************************************************************
			*隐藏其他框架
			******************************************************************/
			try{
				$($this).find("iframe[operate=\"frame\"]").hide();
			}catch(err){}
			/*****************************************************************
			*判断指定的框架是否存在,否则执行创建
			******************************************************************/
			try{
				var Frame = $($this).find("iframe[name=\""+options['name']+"\"]")[0];
				if(Frame==undefined || Frame==null)
				{
					try{
						Frame = document.createElement('iframe');
						$(Frame).attr({'name':options['name'],'frameborder':'0','scrolling':'auto'});
						$(Frame).attr({'operate':'frame'});
						if(options['url']!=undefined && options['url']!='')
						{$(Frame).attr({'src':options['url']});}
						$($this).append(Frame);
					}catch(err){}
				};
			}catch(err){}
			/*****************************************************************
			*制作网页正在加载特效
			******************************************************************/
			if(Frame!=undefined && Frame!=null){
				try{
					var interval = setInterval(function(){
						if(Frame.document!=undefined 
							&& Frame.document!=null 
							&& Frame.document.readyState != "complete")
						{
							
						}else{
							try{
								clearInterval(interval);
								try{$($this).attr('status','complate');}
								catch(err){}
								$(Frame).show();
							}catch(err){}
						}
					},1000);
				}catch(err){}
			}
		}
	};
})(jQuery);
























