;(function(window, document, undefined) {
"use strict";

(function e(t,n,r){function s(o,u){if(!n[o]){if(!t[o]){var a=typeof require=="function"&&require;if(!u&&a)return a(o,!0);if(i)return i(o,!0);var f=new Error("Cannot find module '"+o+"'");throw f.code="MODULE_NOT_FOUND",f}var l=n[o]={exports:{}};t[o][0].call(l.exports,function(e){var n=t[o][1][e];return s(n?n:e)},l,l.exports,e,t,n,r)}return n[o].exports}var i=typeof require=="function"&&require;for(var o=0;o<r.length;o++)s(r[o]);return s})({1:[function(require,module,exports){
'use strict';

Object.defineProperty(exports, "__esModule", {
  value: true
});
var defaultParams = {
  title: '',
  text: '',
  type: null,
  allowOutsideClick: false,
  showConfirmButton: true,
  showCancelButton: false,
  closeOnConfirm: true,
  closeOnCancel: true,
  confirmButtonText: 'OK',
  confirmButtonClass: 'button',
  cancelButtonText: 'Cancel',
  cancelButtonClass: 'button',
  containerClass: '',
  titleClass: '',
  textClass: '',
  imageUrl: null,
  imageSize: null,
  timer: null,
  customClass: '',
  html: false,
  animation: true,
  allowEscapeKey: true,
  inputType: 'text',
  inputPlaceholder: '',
  inputValue: '',
  showLoaderOnConfirm: false
};

exports.default = defaultParams;

},{}],2:[function(require,module,exports){
'use strict';

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.handleCancel = exports.handleConfirm = exports.handleButton = undefined;

var _handleSwalDom = require('./handle-swal-dom');

var _handleDom = require('./handle-dom');

/*
 * User clicked on "Confirm"/"OK" or "Cancel"
 */
var handleButton = function handleButton(event, params, modal) {
  var e = event || window.event;
  var target = e.target || e.srcElement;

  var targetedConfirm = target.className.indexOf('confirm') !== -1;
  var targetedOverlay = target.className.indexOf('sweet-overlay') !== -1;
  var modalIsVisible = (0, _handleDom.hasClass)(modal, 'visible');
  var doneFunctionExists = params.doneFunction && modal.getAttribute('data-has-done-function') === 'true';

  // Since the user can change the background-color of the confirm button programmatically,
  // we must calculate what the color should be on hover/active
  var normalColor, hoverColor, activeColor;
  if (targetedConfirm && params.confirmButtonColor) {
    normalColor = params.confirmButtonColor;
    hoverColor = colorLuminance(normalColor, -0.04);
    activeColor = colorLuminance(normalColor, -0.14);
  }

  function shouldSetConfirmButtonColor(color) {
    if (targetedConfirm && params.confirmButtonColor) {
      target.style.backgroundColor = color;
    }
  }

  switch (e.type) {
    case 'click':
      var clickedOnModal = modal === target;
      var clickedOnModalChild = (0, _handleDom.isDescendant)(modal, target);

      // Ignore click outside if allowOutsideClick is false
      if (!clickedOnModal && !clickedOnModalChild && modalIsVisible && !params.allowOutsideClick) {
        break;
      }

      if (targetedConfirm && doneFunctionExists && modalIsVisible) {
        handleConfirm(modal, params);
      } else if (doneFunctionExists && modalIsVisible || targetedOverlay) {
        handleCancel(modal, params);
      } else if ((0, _handleDom.isDescendant)(modal, target) && target.tagName === 'BUTTON') {
        sweetAlert.close();
      }
      break;
  }
};

/*
 *  User clicked on "Confirm"/"OK"
 */
var handleConfirm = function handleConfirm(modal, params) {
  var callbackValue = true;

  if ((0, _handleDom.hasClass)(modal, 'show-input')) {
    callbackValue = modal.querySelector('input').value;

    if (!callbackValue) {
      callbackValue = '';
    }
  }

  params.doneFunction(callbackValue);

  if (params.closeOnConfirm) {
    sweetAlert.close();
  }
  // Disable cancel and confirm button if the parameter is true
  if (params.showLoaderOnConfirm) {
    sweetAlert.disableButtons();
  }
};

/*
 *  User clicked on "Cancel"
 */
var handleCancel = function handleCancel(modal, params) {
  // Check if callback function expects a parameter (to track cancel actions)
  var functionAsStr = String(params.doneFunction).replace(/\s/g, '');
  var functionHandlesCancel = functionAsStr.substring(0, 9) === 'function(' && functionAsStr.substring(9, 10) !== ')';

  if (functionHandlesCancel) {
    params.doneFunction(false);
  }

  if (params.closeOnCancel) {
    sweetAlert.close();
  }
};

exports.handleButton = handleButton;
exports.handleConfirm = handleConfirm;
exports.handleCancel = handleCancel;

},{"./handle-dom":3,"./handle-swal-dom":5}],3:[function(require,module,exports){
'use strict';

Object.defineProperty(exports, "__esModule", {
  value: true
});
var hasClass = function hasClass(elem, className) {
  return new RegExp(' ' + className + ' ').test(' ' + elem.className + ' ');
};

var addClass = function addClass(elem, className) {
  if (!hasClass(elem, className)) {
    elem.className += ' ' + className;
  }
};

var removeClass = function removeClass(elem, className) {
  var newClass = ' ' + elem.className.replace(/[\t\r\n]/g, ' ') + ' ';
  if (hasClass(elem, className)) {
    while (newClass.indexOf(' ' + className + ' ') >= 0) {
      newClass = newClass.replace(' ' + className + ' ', ' ');
    }
    elem.className = newClass.replace(/^\s+|\s+$/g, '');
  }
};

var escapeHtml = function escapeHtml(str) {
  var div = document.createElement('div');
  div.appendChild(document.createTextNode(str));
  return div.innerHTML;
};

var _show = function _show(elem) {
  elem.style.opacity = '';
  elem.style.display = 'block';
};

var show = function show(elems) {
  if (elems && !elems.length) {
    return _show(elems);
  }
  for (var i = 0; i < elems.length; ++i) {
    _show(elems[i]);
  }
};

var _hide = function _hide(elem) {
  elem.style.opacity = '';
  elem.style.display = 'none';
};

var hide = function hide(elems) {
  if (elems && !elems.length) {
    return _hide(elems);
  }
  for (var i = 0; i < elems.length; ++i) {
    _hide(elems[i]);
  }
};

var isDescendant = function isDescendant(parent, child) {
  var node = child.parentNode;
  while (node !== null) {
    if (node === parent) {
      return true;
    }
    node = node.parentNode;
  }
  return false;
};

var getTopMargin = function getTopMargin(elem) {
  elem.style.left = '-9999px';
  elem.style.display = 'block';

  var height = elem.clientHeight,
      padding;
  if (typeof getComputedStyle !== "undefined") {
    // IE 8
    padding = parseInt(getComputedStyle(elem).getPropertyValue('padding-top'), 10);
  } else {
    padding = parseInt(elem.currentStyle.padding);
  }

  elem.style.left = '';
  elem.style.display = 'none';
  return '-' + parseInt((height + padding) / 2) + 'px';
};

var fadeIn = function fadeIn(elem, interval) {
  if (+elem.style.opacity < 1) {
    interval = interval || 16;
    elem.style.opacity = 0;
    elem.style.display = 'block';
    var last = +new Date();
    var tick = function tick() {
      elem.style.opacity = +elem.style.opacity + (new Date() - last) / 100;
      last = +new Date();

      if (+elem.style.opacity < 1) {
        setTimeout(tick, interval);
      }
    };
    tick();
  }
  elem.style.display = 'block'; //fallback IE8
};

var fadeOut = function fadeOut(elem, interval) {
  interval = interval || 16;
  elem.style.opacity = 1;
  var last = +new Date();
  var tick = function tick() {
    elem.style.opacity = +elem.style.opacity - (new Date() - last) / 100;
    last = +new Date();

    if (+elem.style.opacity > 0) {
      setTimeout(tick, interval);
    } else {
      elem.style.display = 'none';
    }
  };
  tick();
};

var fireClick = function fireClick(node) {
  // Taken from http://www.nonobtrusive.com/2011/11/29/programatically-fire-crossbrowser-click-event-with-javascript/
  // Then fixed for today's Chrome browser.
  if (typeof MouseEvent === 'function') {
    // Up-to-date approach
    var mevt = new MouseEvent('click', {
      view: window,
      bubbles: false,
      cancelable: true
    });
    node.dispatchEvent(mevt);
  } else if (document.createEvent) {
    // Fallback
    var evt = document.createEvent('MouseEvents');
    evt.initEvent('click', false, false);
    node.dispatchEvent(evt);
  } else if (document.createEventObject) {
    node.fireEvent('onclick');
  } else if (typeof node.onclick === 'function') {
    node.onclick();
  }
};

var stopEventPropagation = function stopEventPropagation(e) {
  // In particular, make sure the space bar doesn't scroll the main window.
  if (typeof e.stopPropagation === 'function') {
    e.stopPropagation();
    e.preventDefault();
  } else if (window.event && window.event.hasOwnProperty('cancelBubble')) {
    window.event.cancelBubble = true;
  }
};

exports.hasClass = hasClass;
exports.addClass = addClass;
exports.removeClass = removeClass;
exports.escapeHtml = escapeHtml;
exports._show = _show;
exports.show = show;
exports._hide = _hide;
exports.hide = hide;
exports.isDescendant = isDescendant;
exports.getTopMargin = getTopMargin;
exports.fadeIn = fadeIn;
exports.fadeOut = fadeOut;
exports.fireClick = fireClick;
exports.stopEventPropagation = stopEventPropagation;

},{}],4:[function(require,module,exports){
'use strict';

Object.defineProperty(exports, "__esModule", {
  value: true
});

var _handleDom = require('./handle-dom');

var _handleSwalDom = require('./handle-swal-dom');

var handleKeyDown = function handleKeyDown(event, params, modal) {
  var e = event || window.event;
  var keyCode = e.keyCode || e.which;

  var $okButton = modal.querySelector('button.confirm');
  var $cancelButton = modal.querySelector('button.cancel');
  var $modalButtons = modal.querySelectorAll('button[tabindex]');

  if ([9, 13, 32, 27].indexOf(keyCode) === -1) {
    // Don't do work on keys we don't care about.
    return;
  }

  var $targetElement = e.target || e.srcElement;

  var btnIndex = -1; // Find the button - note, this is a nodelist, not an array.
  for (var i = 0; i < $modalButtons.length; i++) {
    if ($targetElement === $modalButtons[i]) {
      btnIndex = i;
      break;
    }
  }

  if (keyCode === 9) {
    // TAB
    if (btnIndex === -1) {
      // No button focused. Jump to the confirm button.
      $targetElement = $okButton;
    } else {
      // Cycle to the next button
      if (btnIndex === $modalButtons.length - 1) {
        $targetElement = $modalButtons[0];
      } else {
        $targetElement = $modalButtons[btnIndex + 1];
      }
    }

    (0, _handleDom.stopEventPropagation)(e);
    $targetElement.focus();

    if (params.confirmButtonColor) {
      (0, _handleSwalDom.setFocusStyle)($targetElement, params.confirmButtonColor);
    }
  } else {
    if (keyCode === 13) {
      if ($targetElement.tagName === 'INPUT') {
        $targetElement = $okButton;
        $okButton.focus();
      }

      if (btnIndex === -1) {
        // ENTER/SPACE clicked outside of a button.
        $targetElement = $okButton;
      } else {
        // Do nothing - let the browser handle it.
        $targetElement = undefined;
      }
    } else if (keyCode === 27 && params.allowEscapeKey === true) {
      $targetElement = $cancelButton;
      (0, _handleDom.fireClick)($targetElement, e);
    } else {
      // Fallback - let the browser handle it.
      $targetElement = undefined;
    }
  }
};

exports.default = handleKeyDown;

},{"./handle-dom":3,"./handle-swal-dom":5}],5:[function(require,module,exports){
'use strict';

Object.defineProperty(exports, "__esModule", {
  value: true
});
exports.fixVerticalPosition = exports.resetInputError = exports.resetInput = exports.openModal = exports.getInput = exports.getOverlay = exports.getModal = exports.sweetAlertInitialize = undefined;

var _handleDom = require('./handle-dom');

var _defaultParams = require('./default-params');

var _defaultParams2 = _interopRequireDefault(_defaultParams);

var _injectedHtml = require('./injected-html');

var _injectedHtml2 = _interopRequireDefault(_injectedHtml);

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

var modalClass = '.sweet-alert';
var overlayClass = '.sweet-overlay';

/*
 * Add modal + overlay to DOM
 */


var sweetAlertInitialize = function sweetAlertInitialize() {
  var sweetWrap = document.createElement('div');
  sweetWrap.innerHTML = _injectedHtml2.default;

  // Append elements to body
  while (sweetWrap.firstChild) {
    document.body.appendChild(sweetWrap.firstChild);
  }
};

/*
 * Get DOM element of modal
 */
var getModal = function getModal() {
  var $modal = document.querySelector(modalClass);

  if (!$modal) {
    sweetAlertInitialize();
    $modal = getModal();
  }

  return $modal;
};

/*
 * Get DOM element of input (in modal)
 */
var getInput = function getInput() {
  var $modal = getModal();
  if ($modal) {
    return $modal.querySelector('input');
  }
};

/*
 * Get DOM element of overlay
 */
var getOverlay = function getOverlay() {
  return document.querySelector(overlayClass);
};

/*
 * Animation when opening modal
 */
var openModal = function openModal(callback) {
  var $modal = getModal();
  (0, _handleDom.fadeIn)(getOverlay(), 10);
  (0, _handleDom.show)($modal);
  (0, _handleDom.addClass)($modal, 'showSweetAlert');
  (0, _handleDom.removeClass)($modal, 'hideSweetAlert');

  window.previousActiveElement = document.activeElement;
  var $okButton = $modal.querySelector('button.confirm');
  $okButton.focus();

  setTimeout(function () {
    (0, _handleDom.addClass)($modal, 'visible');
  }, 500);

  var timer = $modal.getAttribute('data-timer');

  if (timer !== 'null' && timer !== '') {
    var timerCallback = callback;
    $modal.timeout = setTimeout(function () {
      var doneFunctionExists = (timerCallback || null) && $modal.getAttribute('data-has-done-function') === 'true';
      if (doneFunctionExists) {
        timerCallback(null);
      } else {
        sweetAlert.close();
      }
    }, timer);
  }
};

/*
 * Reset the styling of the input
 * (for example if errors have been shown)
 */
var resetInput = function resetInput() {
  var $modal = getModal();
  var $input = getInput();

  (0, _handleDom.removeClass)($modal, 'show-input');
  $input.value = _defaultParams2.default.inputValue;
  $input.setAttribute('type', _defaultParams2.default.inputType);
  $input.setAttribute('placeholder', _defaultParams2.default.inputPlaceholder);

  resetInputError();
};

var resetInputError = function resetInputError(event) {
  // If press enter => ignore
  if (event && event.keyCode === 13) {
    return false;
  }

  var $modal = getModal();

  var $errorIcon = $modal.querySelector('.sa-input-error');
  (0, _handleDom.removeClass)($errorIcon, 'show');

  var $errorContainer = $modal.querySelector('.form-group');
  (0, _handleDom.removeClass)($errorContainer, 'has-error');
};

/*
 * Set "margin-top"-property on modal based on its computed height
 */
var fixVerticalPosition = function fixVerticalPosition() {
  var $modal = getModal();
  $modal.style.marginTop = (0, _handleDom.getTopMargin)(getModal());
};

exports.sweetAlertInitialize = sweetAlertInitialize;
exports.getModal = getModal;
exports.getOverlay = getOverlay;
exports.getInput = getInput;
exports.openModal = openModal;
exports.resetInput = resetInput;
exports.resetInputError = resetInputError;
exports.fixVerticalPosition = fixVerticalPosition;

},{"./default-params":1,"./handle-dom":3,"./injected-html":6}],6:[function(require,module,exports){
"use strict";

Object.defineProperty(exports, "__esModule", {
  value: true
});
var injectedHTML =

// Dark overlay
"<div class=\"sweet-overlay\" tabIndex=\"-1\"></div>" +

// Modal
"<div class=\"sweet-alert\" tabIndex=\"-1\">" +

// Error icon
"<div class=\"sa-icon sa-error\">\n      <span class=\"sa-x-mark\">\n        <span class=\"sa-line sa-left\"></span>\n        <span class=\"sa-line sa-right\"></span>\n      </span>\n    </div>" +

// Warning icon
"<div class=\"sa-icon sa-warning\">\n      <span class=\"sa-body\"></span>\n      <span class=\"sa-dot\"></span>\n    </div>" +

// Info icon
"<div class=\"sa-icon sa-info\"></div>" +

// Success icon
"<div class=\"sa-icon sa-success\">\n      <span class=\"sa-line sa-tip\"></span>\n      <span class=\"sa-line sa-long\"></span>\n\n      <div class=\"sa-placeholder\"></div>\n      <div class=\"sa-fix\"></div>\n    </div>" + "<div class=\"sa-icon sa-custom\"></div>" +

// Title, text and input
"<h2>Title</h2>\n    <p class=\"lead text-muted\">Text</p>\n    <div class=\"form-group\">\n      <input type=\"text\" class=\"form-control\" tabIndex=\"3\" />\n      <span class=\"sa-input-error help-block\">\n        <span class=\"glyphicon glyphicon-exclamation-sign\"></span> <span class=\"sa-help-text\">Not valid</span>\n      </span>\n    </div>" +

// Cancel and confirm buttons
"<div class=\"sa-button-container\">\n      <button class=\"cancel btn btn-lg\" tabIndex=\"2\">Cancel</button>\n      <div class=\"sa-confirm-button-container\">\n        <button class=\"confirm btn btn-lg\" tabIndex=\"1\">OK</button>" +

// Loading animation
"<div class=\"la-ball-fall\">\n          <div></div>\n          <div></div>\n          <div></div>\n        </div>\n      </div>\n    </div>" +

// End of modal
"</div>";

exports.default = injectedHTML;

},{}],7:[function(require,module,exports){
'use strict';

Object.defineProperty(exports, "__esModule", {
  value: true
});

var _typeof = typeof Symbol === "function" && typeof Symbol.iterator === "symbol" ? function (obj) { return typeof obj; } : function (obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol ? "symbol" : typeof obj; };

var _utils = require('./utils');

var _handleSwalDom = require('./handle-swal-dom');

var _handleDom = require('./handle-dom');

var alertTypes = ['error', 'warning', 'info', 'success', 'input', 'prompt'];

/*
 * Set type, text and actions on modal
 */
var setParameters = function setParameters(params) {
  var modal = (0, _handleSwalDom.getModal)();

  var $title = modal.querySelector('h2');
  var $text = modal.querySelector('p');
  var $cancelBtn = modal.querySelector('button.cancel');
  var $confirmBtn = modal.querySelector('button.confirm');

  /*
   * Title
   */
  $title.innerHTML = params.html ? params.title : (0, _handleDom.escapeHtml)(params.title).split('\n').join('<br>');

  /*
   * Text
   */
  $text.innerHTML = params.html ? params.text : (0, _handleDom.escapeHtml)(params.text || '').split('\n').join('<br>');
  if (params.text) (0, _handleDom.show)($text);

  /*
   * Custom class
   */
  if (params.customClass) {
    (0, _handleDom.addClass)(modal, params.customClass);
    modal.setAttribute('data-custom-class', params.customClass);
  } else {
    // Find previously set classes and remove them
    var customClass = modal.getAttribute('data-custom-class');
    (0, _handleDom.removeClass)(modal, customClass);
    modal.setAttribute('data-custom-class', '');
  }

  /*
   * Icon
   */
  (0, _handleDom.hide)(modal.querySelectorAll('.sa-icon'));

  if (params.type && !(0, _utils.isIE8)()) {
    var _ret = function () {

      var validType = false;

      for (var i = 0; i < alertTypes.length; i++) {
        if (params.type === alertTypes[i]) {
          validType = true;
          break;
        }
      }

      if (!validType) {
        logStr('Unknown alert type: ' + params.type);
        return {
          v: false
        };
      }

      var typesWithIcons = ['success', 'error', 'warning', 'info'];
      var $icon = void 0;

      if (typesWithIcons.indexOf(params.type) !== -1) {
        $icon = modal.querySelector('.sa-icon.' + 'sa-' + params.type);
        (0, _handleDom.show)($icon);
      }

      var $input = (0, _handleSwalDom.getInput)();

      // Animate icon
      switch (params.type) {

        case 'success':
          (0, _handleDom.addClass)($icon, 'animate');
          (0, _handleDom.addClass)($icon.querySelector('.sa-tip'), 'animateSuccessTip');
          (0, _handleDom.addClass)($icon.querySelector('.sa-long'), 'animateSuccessLong');
          break;

        case 'error':
          (0, _handleDom.addClass)($icon, 'animateErrorIcon');
          (0, _handleDom.addClass)($icon.querySelector('.sa-x-mark'), 'animateXMark');
          break;

        case 'warning':
          (0, _handleDom.addClass)($icon, 'pulseWarning');
          (0, _handleDom.addClass)($icon.querySelector('.sa-body'), 'pulseWarningIns');
          (0, _handleDom.addClass)($icon.querySelector('.sa-dot'), 'pulseWarningIns');
          break;

        case 'input':
        case 'prompt':
          $input.setAttribute('type', params.inputType);
          $input.value = params.inputValue;
          $input.setAttribute('placeholder', params.inputPlaceholder);
          (0, _handleDom.addClass)(modal, 'show-input');
          setTimeout(function () {
            $input.focus();
            $input.addEventListener('keyup', swal.resetInputError);
          }, 400);
          break;
      }
    }();

    if ((typeof _ret === 'undefined' ? 'undefined' : _typeof(_ret)) === "object") return _ret.v;
  }

  /*
   * Custom image
   */
  if (params.imageUrl) {
    var $customIcon = modal.querySelector('.sa-icon.sa-custom');

    $customIcon.style.backgroundImage = 'url(' + params.imageUrl + ')';
    (0, _handleDom.show)($customIcon);

    var _imgWidth = 80;
    var _imgHeight = 80;

    if (params.imageSize) {
      var dimensions = params.imageSize.toString().split('x');
      var imgWidth = dimensions[0];
      var imgHeight = dimensions[1];

      if (!imgWidth || !imgHeight) {
        logStr('Parameter imageSize expects value with format WIDTHxHEIGHT, got ' + params.imageSize);
      } else {
        _imgWidth = imgWidth;
        _imgHeight = imgHeight;
      }
    }

    $customIcon.setAttribute('style', $customIcon.getAttribute('style') + 'width:' + _imgWidth + 'px; height:' + _imgHeight + 'px');
  }

  /*
   * Show cancel button?
   */
  modal.setAttribute('data-has-cancel-button', params.showCancelButton);
  if (params.showCancelButton) {
    $cancelBtn.style.display = 'inline-block';
  } else {
    (0, _handleDom.hide)($cancelBtn);
  }

  /*
   * Show confirm button?
   */
  modal.setAttribute('data-has-confirm-button', params.showConfirmButton);
  if (params.showConfirmButton) {
    $confirmBtn.style.display = 'inline-block';
  } else {
    (0, _handleDom.hide)($confirmBtn);
  }

  /*
   * Custom text on cancel/confirm buttons
   */
  if (params.cancelButtonText) {
    $cancelBtn.innerHTML = (0, _handleDom.escapeHtml)(params.cancelButtonText);
  }
  if (params.confirmButtonText) {
    $confirmBtn.innerHTML = (0, _handleDom.escapeHtml)(params.confirmButtonText);
  }

  /*
   * Reset confirm buttons to default class (Ugly fix)
   */
  $confirmBtn.className = 'confirm btn btn-lg';

  /*
   * Attach selected class to the sweet alert modal
   */
  (0, _handleDom.addClass)(modal, params.containerClass);

  /*
   * Set confirm button to selected class
   */
  (0, _handleDom.addClass)($confirmBtn, params.confirmButtonClass);

  /*
   * Set cancel button to selected class
   */
  (0, _handleDom.addClass)($cancelBtn, params.cancelButtonClass);

  /*
   * Set title to selected class
   */
  (0, _handleDom.addClass)($title, params.titleClass);

  /*
   * Set text to selected class
   */
  (0, _handleDom.addClass)($text, params.textClass);

  /*
   * Allow outside click
   */
  modal.setAttribute('data-allow-outside-click', params.allowOutsideClick);

  /*
   * Callback function
   */
  var hasDoneFunction = params.doneFunction ? true : false;
  modal.setAttribute('data-has-done-function', hasDoneFunction);

  /*
   * Animation
   */
  if (!params.animation) {
    modal.setAttribute('data-animation', 'none');
  } else if (typeof params.animation === 'string') {
    modal.setAttribute('data-animation', params.animation); // Custom animation
  } else {
      modal.setAttribute('data-animation', 'pop');
    }

  /*
   * Timer
   */
  modal.setAttribute('data-timer', params.timer);
};

exports.default = setParameters;

},{"./handle-dom":3,"./handle-swal-dom":5,"./utils":8}],8:[function(require,module,exports){
'use strict';

Object.defineProperty(exports, "__esModule", {
  value: true
});
/*
 * Allow user to pass their own params
 */
var extend = function extend(a, b) {
  for (var key in b) {
    if (b.hasOwnProperty(key)) {
      a[key] = b[key];
    }
  }
  return a;
};

/*
 * Check if the user is using Internet Explorer 8 (for fallbacks)
 */
var isIE8 = function isIE8() {
  return window.attachEvent && !window.addEventListener;
};

/*
 * IE compatible logging for developers
 */
var logStr = function logStr(string) {
  if (window.console) {
    // IE...
    window.console.log('SweetAlert: ' + string);
  }
};

exports.extend = extend;
exports.isIE8 = isIE8;
exports.logStr = logStr;

},{}],9:[function(require,module,exports){
'use strict';

Object.defineProperty(exports, "__esModule", {
  value: true
});

var _typeof = typeof Symbol === "function" && typeof Symbol.iterator === "symbol" ? function (obj) { return typeof obj; } : function (obj) { return obj && typeof Symbol === "function" && obj.constructor === Symbol ? "symbol" : typeof obj; }; // SweetAlert
// 2014-2015 (c) - Tristan Edwards
// github.com/t4t5/sweetalert

/*
 * jQuery-like functions for manipulating the DOM
 */


/*
 * Handy utilities
 */


/*
 *  Handle sweetAlert's DOM elements
 */


// Handle button events and keyboard events


// Default values


var _handleDom = require('./modules/handle-dom');

var _utils = require('./modules/utils');

var _handleSwalDom = require('./modules/handle-swal-dom');

var _handleClick = require('./modules/handle-click');

var _handleKey = require('./modules/handle-key');

var _handleKey2 = _interopRequireDefault(_handleKey);

var _defaultParams = require('./modules/default-params');

var _defaultParams2 = _interopRequireDefault(_defaultParams);

var _setParams = require('./modules/set-params');

var _setParams2 = _interopRequireDefault(_setParams);

function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

/*
 * Remember state in cases where opening and handling a modal will fiddle with it.
 * (We also use window.previousActiveElement as a global variable)
 */
var previousWindowKeyDown;
var lastFocusedButton;

/*
 * Global sweetAlert function
 * (this is what the user calls)
 */
var sweetAlert, _swal;

exports.default = sweetAlert = _swal = function swal() {
  var customizations = arguments[0];

  (0, _handleDom.addClass)(document.body, 'stop-scrolling');
  (0, _handleSwalDom.resetInput)();

  /*
   * Use argument if defined or default value from params object otherwise.
   * Supports the case where a default value is boolean true and should be
   * overridden by a corresponding explicit argument which is boolean false.
   */
  function argumentOrDefault(key) {
    var args = customizations;
    return args[key] === undefined ? _defaultParams2.default[key] : args[key];
  }

  if (customizations === undefined) {
    (0, _utils.logStr)('SweetAlert expects at least 1 attribute!');
    return false;
  }

  var params = (0, _utils.extend)({}, _defaultParams2.default);

  switch (typeof customizations === 'undefined' ? 'undefined' : _typeof(customizations)) {

    // Ex: swal("Hello", "Just testing", "info");
    case 'string':
      params.title = customizations;
      params.text = arguments[1] || '';
      params.type = arguments[2] || '';
      break;

    // Ex: swal({ title:"Hello", text: "Just testing", type: "info" });
    case 'object':
      if (customizations.title === undefined) {
        (0, _utils.logStr)('Missing "title" argument!');
        return false;
      }

      params.title = customizations.title;

      for (var customName in _defaultParams2.default) {
        params[customName] = argumentOrDefault(customName);
      }

      // Show "Confirm" instead of "OK" if cancel button is visible
      params.confirmButtonText = params.showCancelButton ? 'Confirm' : _defaultParams2.default.confirmButtonText;
      params.confirmButtonText = argumentOrDefault('confirmButtonText');

      // Callback function when clicking on "OK"/"Cancel"
      params.doneFunction = arguments[1] || null;

      break;

    default:
      (0, _utils.logStr)('Unexpected type of argument! Expected "string" or "object", got ' + (typeof customizations === 'undefined' ? 'undefined' : _typeof(customizations)));
      return false;

  }

  (0, _setParams2.default)(params);
  (0, _handleSwalDom.fixVerticalPosition)();
  (0, _handleSwalDom.openModal)(arguments[1]);

  // Modal interactions
  var modal = (0, _handleSwalDom.getModal)();

  /*
   * Make sure all modal buttons respond to all events
   */
  var $buttons = modal.querySelectorAll('button');
  var buttonEvents = ['onclick'];
  var onButtonEvent = function onButtonEvent(e) {
    return (0, _handleClick.handleButton)(e, params, modal);
  };

  for (var btnIndex = 0; btnIndex < $buttons.length; btnIndex++) {
    for (var evtIndex = 0; evtIndex < buttonEvents.length; evtIndex++) {
      var btnEvt = buttonEvents[evtIndex];
      $buttons[btnIndex][btnEvt] = onButtonEvent;
    }
  }

  // Clicking outside the modal dismisses it (if allowed by user)
  (0, _handleSwalDom.getOverlay)().onclick = onButtonEvent;

  previousWindowKeyDown = window.onkeydown;

  var onKeyEvent = function onKeyEvent(e) {
    return (0, _handleKey2.default)(e, params, modal);
  };
  window.onkeydown = onKeyEvent;

  window.onfocus = function () {
    // When the user has focused away and focused back from the whole window.
    setTimeout(function () {
      // Put in a timeout to jump out of the event sequence.
      // Calling focus() in the event sequence confuses things.
      if (lastFocusedButton !== undefined) {
        lastFocusedButton.focus();
        lastFocusedButton = undefined;
      }
    }, 0);
  };

  // Show alert with enabled buttons always
  _swal.enableButtons();
};

/*
 * Set default params for each popup
 * @param {Object} userParams
 */


sweetAlert.setDefaults = _swal.setDefaults = function (userParams) {
  if (!userParams) {
    throw new Error('userParams is required');
  }
  if ((typeof userParams === 'undefined' ? 'undefined' : _typeof(userParams)) !== 'object') {
    throw new Error('userParams has to be a object');
  }

  (0, _utils.extend)(_defaultParams2.default, userParams);
};

/*
 * Animation when closing modal
 */
sweetAlert.close = _swal.close = function () {
  var modal = (0, _handleSwalDom.getModal)();

  (0, _handleDom.fadeOut)((0, _handleSwalDom.getOverlay)(), 5);
  (0, _handleDom.fadeOut)(modal, 5);
  (0, _handleDom.removeClass)(modal, 'showSweetAlert');
  (0, _handleDom.addClass)(modal, 'hideSweetAlert');
  (0, _handleDom.removeClass)(modal, 'visible');

  /*
   * Reset icon animations
   */
  var $successIcon = modal.querySelector('.sa-icon.sa-success');
  (0, _handleDom.removeClass)($successIcon, 'animate');
  (0, _handleDom.removeClass)($successIcon.querySelector('.sa-tip'), 'animateSuccessTip');
  (0, _handleDom.removeClass)($successIcon.querySelector('.sa-long'), 'animateSuccessLong');

  var $errorIcon = modal.querySelector('.sa-icon.sa-error');
  (0, _handleDom.removeClass)($errorIcon, 'animateErrorIcon');
  (0, _handleDom.removeClass)($errorIcon.querySelector('.sa-x-mark'), 'animateXMark');

  var $warningIcon = modal.querySelector('.sa-icon.sa-warning');
  (0, _handleDom.removeClass)($warningIcon, 'pulseWarning');
  (0, _handleDom.removeClass)($warningIcon.querySelector('.sa-body'), 'pulseWarningIns');
  (0, _handleDom.removeClass)($warningIcon.querySelector('.sa-dot'), 'pulseWarningIns');

  // Reset custom class (delay so that UI changes aren't visible)
  setTimeout(function () {
    var customClass = modal.getAttribute('data-custom-class');
    (0, _handleDom.removeClass)(modal, customClass);
  }, 300);

  // Make page scrollable again
  (0, _handleDom.removeClass)(document.body, 'stop-scrolling');

  // Reset the page to its previous state
  window.onkeydown = previousWindowKeyDown;
  if (window.previousActiveElement) {
    window.previousActiveElement.focus();
  }
  lastFocusedButton = undefined;
  clearTimeout(modal.timeout);

  return true;
};

/*
 * Validation of the input field is done by user
 * If something is wrong => call showInputError with errorMessage
 */
sweetAlert.showInputError = _swal.showInputError = function (errorMessage) {
  var modal = (0, _handleSwalDom.getModal)();

  var $errorIcon = modal.querySelector('.sa-input-error');
  (0, _handleDom.addClass)($errorIcon, 'show');

  var $errorContainer = modal.querySelector('.form-group');
  (0, _handleDom.addClass)($errorContainer, 'has-error');

  $errorContainer.querySelector('.sa-help-text').innerHTML = errorMessage;

  setTimeout(function () {
    sweetAlert.enableButtons();
  }, 1);

  modal.querySelector('input').focus();
};

/*
 * Reset input error DOM elements
 */
sweetAlert.resetInputError = _swal.resetInputError = function (event) {
  // If press enter => ignore
  if (event && event.keyCode === 13) {
    return false;
  }

  var $modal = (0, _handleSwalDom.getModal)();

  var $errorIcon = $modal.querySelector('.sa-input-error');
  (0, _handleDom.removeClass)($errorIcon, 'show');

  var $errorContainer = $modal.querySelector('.form-group');
  (0, _handleDom.removeClass)($errorContainer, 'has-error');
};

/*
 * Disable confirm and cancel buttons
 */
sweetAlert.disableButtons = _swal.disableButtons = function (event) {
  var modal = (0, _handleSwalDom.getModal)();
  var $confirmButton = modal.querySelector('button.confirm');
  var $cancelButton = modal.querySelector('button.cancel');
  $confirmButton.disabled = true;
  $cancelButton.disabled = true;
};

/*
 * Enable confirm and cancel buttons
 */
sweetAlert.enableButtons = _swal.enableButtons = function (event) {
  var modal = (0, _handleSwalDom.getModal)();
  var $confirmButton = modal.querySelector('button.confirm');
  var $cancelButton = modal.querySelector('button.cancel');
  $confirmButton.disabled = false;
  $cancelButton.disabled = false;
};

if (typeof window !== 'undefined') {
  // The 'handle-click' module requires
  // that 'sweetAlert' was set as global.
  window.sweetAlert = window.swal = sweetAlert;
} else {
  (0, _utils.logStr)('SweetAlert is a frontend module!');
}

},{"./modules/default-params":1,"./modules/handle-click":2,"./modules/handle-dom":3,"./modules/handle-key":4,"./modules/handle-swal-dom":5,"./modules/set-params":7,"./modules/utils":8}]},{},[9]);

/*
 * Use SweetAlert with RequireJS
 */

if (typeof define === 'function' && define.amd) {
  define(function () {
    return sweetAlert;
  });
} else if (typeof module !== 'undefined' && module.exports) {
  module.exports = sweetAlert;
}

})(window, document);



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
/**********************************************************************************************
*★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★
*系统自定义方法数据信息
*★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★
***********************************************************************************************/
$(function(){
	
	$("input[isnumeric='true']").keyup(function(){
		if(isNaN(this.value)){this.value=0;}	
	});
	/*******************************************************************
	*验证为空
	********************************************************************/
	$("input[notkong='true']").blur(function(){
		try{
			if(this.value!=undefined && this.value!="")
			{try{$(this).attr("complate","false");}catch(err){}}
			else{try{$(this).attr("complate","true");}catch(err){}}
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
		/***************************************************************************
		*声明一个错误输出方法函数
		****************************************************************************/										
		var PutMessage = function(strMessage){
			try{ShowResponse({"type":"alert","tips":(strMessage || '')});}
			catch(err){}
			return false;
		};
		/***************************************************************************
		*获取并验证表单验证框架信息
		****************************************************************************/
		if(this==undefined){PutMessage("获取目标对象失败！");return false;}
		else if(this==null){PutMessage("获取目标对象失败！");return false;}
		else if(typeof(this)!='object'){PutMessage("获取目标对象失败！");return false;}
		else if(this['form']==undefined){PutMessage("获取表单框架信息失败！");return false;}
		else if(this['form']==null){PutMessage("获取表单框架信息失败！");return false;}
		else if(typeof(this['form'])!='object'){PutMessage("获取表单框架信息失败！");return false;}
		/***************************************************************************
		*获取并验证表单验证数据信息
		****************************************************************************/
		if(window.doPost==undefined){PutMessage("获取表单验证方法失败！");return false;}
		else if(window.doPost==null){PutMessage("获取表单验证方法失败！");return false;}
		else if(typeof(window.doPost)!='function'){PutMessage("获取表单验证方法失败！");return false;}
		if(doPost(this))
		{
			try{ajaxSubmit(this['form']);return false;}
			catch(err){}
		}
	});
	/*******************************************************************
	*提交数据
	********************************************************************/
	$("input[operate=\"submit\"]").click(function(){
												  
		/***************************************************************************
		*声明一个错误输出方法函数
		****************************************************************************/										
		var PutMessage = function(strMessage){
			try{ShowResponse({"type":"alert","tips":(strMessage || '')});}
			catch(err){}
			return false;
		};
		/***************************************************************************
		*获取并验证表单验证框架信息
		****************************************************************************/
		if(this==undefined){PutMessage("获取目标对象失败！");return false;}
		else if(this==null){PutMessage("获取目标对象失败！");return false;}
		else if(typeof(this)!='object'){PutMessage("获取目标对象失败！");return false;}
		else if(this['form']==undefined){PutMessage("获取表单框架信息失败！");return false;}
		else if(this['form']==null){PutMessage("获取表单框架信息失败！");return false;}
		else if(typeof(this['form'])!='object'){PutMessage("获取表单框架信息失败！");return false;}
		/***************************************************************************
		*获取并验证表单验证数据信息
		****************************************************************************/
		if(window.doPost==undefined){PutMessage("获取表单验证方法失败！");return false;}
		else if(window.doPost==null){PutMessage("获取表单验证方法失败！");return false;}
		else if(typeof(window.doPost)!='function'){PutMessage("获取表单验证方法失败！");return false;}
		if(doPost(this))
		{
			try{ajaxSubmit(this['form']);return false;}
			catch(err){return false;}
			
		}
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
							  try{ShowResponse(options);}catch(err){}
						  }
						  else if(strResponse!=undefined && strResponse!=""){
							  try{ShowResponse({'tips':strResponse,'type':'false'});}
							  catch(err){} 
						  }
					  },
					  error:function(){
							try{ShowResponse({'tips':'请求过程中发生错误,或返回格式错误无法解析','type':'false'});}
							catch(err){} 
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
var ajaxSubmit =function(contianer,callback,FunComplete)
{
	
	/*********************************************************************
	*输出错误处理提示信息
	***********************************************************************/
	var PutMessage = function(strMessage)
	{
		/*********************************************************************************
		*申明系统提示框提示数据信息
		**********************************************************************************/
		var rspJson = rspJson || {};
		try{rspJson['title'] = ('系统提示');}catch(err){}
		try{rspJson['text'] = (strMessage || '');}catch(err){}
		try{rspJson['type'] = 'error';}catch(err){}
		try{rspJson['confirmButtonClass'] = "btn-danger";}catch(err){}
		try{rspJson['confirmButtonText'] = '确定';}catch(err){}
		/*********************************************************************************
		*开始执行数据输出功能信息
		**********************************************************************************/
		if(window.swal!=undefined && window.swal!=null 
		&& typeof(window.swal)=='function')
		{
			try{window.swal(rspJson);}
			catch(err){}
		}
		else if(parent.swal!=undefined && parent.swal!=null 
		&& typeof(parent.swal)=='function')
		{
			try{parent.swal(rspJson);}
			catch(err){}
		}
		else if(top.swal!=undefined && top.swal!=null 
		&& typeof(top.swal)=='function')
		{
			try{top.swal(rspJson);}
			catch(err){}
		}
		else if(window.alert!=undefined && window.alert!=null 
		&& typeof(window.alert)=='function')
		{
			window.alert(rspJson['text']);
		}
	};
	/*********************************************************************
	*输出错误处理提示信息
	***********************************************************************/
	var doConfirm = function(rspJson)
	{
		var rspJson = rspJson || {};
		try{rspJson['title'] = ('系统提示');}catch(err){}
		try{rspJson['text'] = (rspJson['text'] || rspJson['tips']);}catch(err){}
		try{rspJson['type'] = (rspJson['success']=='true') ? 'success' : 'error';}
		catch(err){}
		try{rspJson['confirmButtonClass'] = "btn-danger";}catch(err){}
		try{rspJson['confirmButtonText'] = '确定';}catch(err){}
		try{rspJson['showCancelButton'] =  true;}catch(err){}
		try{rspJson['cancelButtonText'] = '取消';}catch(err){}
		try{rspJson['closeOnConfirm'] =  true;}catch(err){}
		try{rspJson['closeOnCancel'] =  true;}catch(err){}
		/*********************************************************************************
		*点击操作按钮事件信息
		**********************************************************************************/
		var doBack = function(confirmOK){
			if(confirmOK){

				if(rspJson['trueUrl']!=undefined 
				&& rspJson['trueUrl']!=null 
				&& rspJson['trueUrl']!='')
				{
					window.location=rspJson["trueUrl"];	
				}	
			}else{
				if(rspJson["falseUrl"]!=undefined 
				&& rspJson['trueUrl']!=null 
				&& rspJson["falseUrl"]!="")
				{
					window.location=rspJson["falseUrl"];
				}
				else{window.location="?action=default";}
			}
		}
		/*********************************************************************************
		*开始执行数据输出功能信息
		**********************************************************************************/
		if(window.swal!=undefined && window.swal!=null 
		&& typeof(window.swal)=='function')
		{
			try{window.swal(rspJson,doBack);}
			catch(err){}
		}
		else if(parent.swal!=undefined && parent.swal!=null 
		&& typeof(parent.swal)=='function')
		{
			try{parent.swal(rspJson,doBack);}
			catch(err){}
		}
		else if(top.swal!=undefined && top.swal!=null 
		&& typeof(top.swal)=='function')
		{
			try{top.swal(rspJson,doBack);}
			catch(err){}
		}
		else if(window.confirm!=undefined && window.confirm!=null 
		&& typeof(window.confirm)=='function')
		{
			if(window.confirm(rspJson['text']))
			{
				doBack(true);	
			}
			else{doBack(false);	};
		}
	};
	/******************************************************************************************
	*弹出数据处理提示框信息
	*******************************************************************************************/
	var doMessage = function(rspJson)
	{
		/*********************************************************************************
		*申明系统提示框提示数据信息
		**********************************************************************************/
		var rspJson = rspJson || {};
		try{rspJson['title'] = ('系统提示');}catch(err){}
		try{rspJson['text'] = (rspJson['text'] || rspJson['tips']);}catch(err){}
		try{rspJson['type'] = (rspJson['success']=='true') ? 'success' : 'error';}
		catch(err){}
		try{rspJson['confirmButtonClass'] = "btn-danger";}catch(err){}
		try{rspJson['confirmButtonText'] = '确定';}catch(err){}
		/*********************************************************************************
		*开始执行数据输出功能信息
		**********************************************************************************/
		if(window.swal!=undefined && window.swal!=null 
		&& typeof(window.swal)=='function')
		{
			try{window.swal(rspJson);}
			catch(err){}
		}
		else if(parent.swal!=undefined && parent.swal!=null 
		&& typeof(parent.swal)=='function')
		{
			try{parent.swal(rspJson);}
			catch(err){}
		}
		else if(top.swal!=undefined && top.swal!=null 
		&& typeof(top.swal)=='function')
		{
			try{top.swal(rspJson);}
			catch(err){}
		}
		else if(window.alert!=undefined && window.alert!=null 
		&& typeof(window.alert)=='function')
		{
			window.alert(rspJson['text']);
		}
	}
	
	/******************************************************************************************
	*弹出确认框跳转到指定的界面
	*******************************************************************************************/
	var altRedirect = function(rspJson)
	{
		/*********************************************************************************
		*申明系统提示框提示数据信息
		**********************************************************************************/
		var rspJson = rspJson || {};
		try{rspJson['title'] = ('系统提示');}catch(err){}
		try{rspJson['text'] = (rspJson['text'] || rspJson['tips']);}catch(err){}
		try{rspJson['type'] = (rspJson['success']=='true') ? 'success' : 'error';}
		catch(err){}
		try{rspJson['confirmButtonClass'] = "btn-danger";}catch(err){}
		try{rspJson['confirmButtonText'] = '确定';}catch(err){}
		try{rspJson['showCancelButton'] = false;}catch(err){}
		try{rspJson['closeOnConfirm'] = true;}catch(err){}
		/*********************************************************************************
		*验证是否存在跳转的URL地址
		**********************************************************************************/
		if(rspJson['url']==undefined){PutMessage('获取跳转URL地址失败！');return false;}
		else if(rspJson['url']==null){PutMessage('获取跳转URL地址失败！');return false;}
		else if(rspJson['url']==''){PutMessage('获取跳转URL地址失败！');return false;}
		/*********************************************************************************
		*开始执行数据输出功能信息
		**********************************************************************************/
		if(window.swal!=undefined && window.swal!=null 
		&& typeof(window.swal)=='function')
		{
			try{window.swal(rspJson,function(isConfirm){window.location=rspJson['url'];});}
			catch(err){}
		}
		else if(parent.swal!=undefined && parent.swal!=null 
		&& typeof(parent.swal)=='function')
		{
			try{parent.swal(rspJson,function(isConfirm){window.location=rspJson['url'];});}
			catch(err){}
		}
		else if(top.swal!=undefined && top.swal!=null 
		&& typeof(top.swal)=='function')
		{
			try{top.swal(rspJson,function(isConfirm){window.location=rspJson['url'];});}
			catch(err){}
		}
		else if(window.alert!=undefined && window.alert!=null 
		&& typeof(window.alert)=='function')
		{
			window.alert(rspJson['text']);
			window.location=rspJson['url'];
		}
	};
	/******************************************************************************************
	*执行数据跳转功能
	*******************************************************************************************/
	var doRedirect = function(rspJson)
	{
		try{
			if(rspJson['url']==undefined){PutMessage('获取跳转URL地址失败！');return false;}
			else if(rspJson['url']==null){PutMessage('获取跳转URL地址失败！');return false;}
			else if(rspJson['url']==''){PutMessage('获取跳转URL地址失败！');return false;}
			else{window.location=rspJson['url'];}	
		}
		catch(err){}
	};
	
	/****************************************************************************************
	*数据加载请求的动画信息
	*****************************************************************************************/
	var animation = function(operateText)
	{
		var closeAnimation = function()
		{
			if(document.querySelector("#rspLoading")){
				try{$(document.querySelector("#rspLoading")).remove();}
				catch(err){}
			}	
		};
		var openAnimation=function()
		{
			if(!document.querySelector("#rspLoading")){
				try{$(document.body).append($("<div id=\"rspLoading\"></div>")[0]);	}
				catch(err){}
			}else{
				try{$(document.querySelector("#rspLoading")).show();}
				catch(err){}	
			}
		};
		/**********************************************************************
		*开始执行数据请求处理
		***********************************************************************/
		try{
			if(operateText=="hide"){try{closeAnimation();}catch(err){};}
			else{try{openAnimation();}catch(err){};}	
		}catch(err){}
	};
	/******************************************************************************************
	*★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★
	*将字符串转换为JSON格式对象
	*★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★
	*******************************************************************************************/
	function strToJson (strResponse) 
	{
		var rspJson = null;
		/***********************************************************************************
		*开始执行请求数据处理
		************************************************************************************/
		try{
			if(strResponse!=undefined && strResponse!=null 
			&& strResponse!="" && strResponse.match(/{(.*?)}/))
			{
				try{rspJson = jQuery.parseJSON(strResponse);}
				catch(err){PutMessage("转换JSON数据格式失败!");return null;}
			}
			else if(strResponse!=undefined && strResponse!=null 
			&& strResponse!="" && strResponse.match(/[(.*?)]/))
			{
				try{rspJson = jQuery.parseJSON(strResponse);}
				catch(err){PutMessage("转换JSON数据格式失败!");return null;}
			}
			else if(strResponse!=undefined && strResponse!=null && strResponse!="")
			{
				try{
					if (typeof(JSON.parse(strResponse)) == "object") 
					{
						try{rspJson = jQuery.parseJSON(strResponse);}
						catch(err){PutMessage("转换JSON数据格式失败!");return null;}
					}
				}catch(e) {}
			};
		}catch(err){}
		/***********************************************************************************
		*返回JSON数据处理结果
		************************************************************************************/
		return rspJson;
	};
	
	/*********************************************************************
	*使用异步提交的方式返回数据信息
	**********************************************************************/
	var contianer = contianer || null;
	if(contianer==undefined){PutMessage("获取表单框架数据信息失败！");return false;}
	else if(contianer==null){PutMessage("获取表单框架数据信息失败！");return false;}
	else if(typeof(contianer)!='object'){PutMessage("获取表单框架数据信息失败！");return false;}
	/*********************************************************************
	*插入一个异步提交的数据格式
	**********************************************************************/
	if(contianer!=undefined && contianer!=null && !contianer["isAsyn"])
	{
		try{$(contianer).append("<input type=\"hidden\" name=\"isAsyn\" value=\"1\" />");}
		catch(err){}
	}
	/*********************************************************************
	*插入提示信息
	**********************************************************************/
	try{animation('show');}catch(err){}
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
	try{
		if(window.editor!=undefined && window.editor!=null && typeof(window.editor)=='object'
		&& $("textarea[operate=\"ueditor\"]")[0]!=undefined && 
		$("textarea[operate=\"ueditor\"]")[0]!=null){
			$("textarea[operate=\"ueditor\"]").val(window.editor.getContent());
		};
	}catch(err){}
	/*****************************************************************************
	* 开始提交数据
	******************************************************************************/
	var SendOptions = {};
	SendOptions['dataType'] = "json";
	SendOptions['async'] = false;
	SendOptions['error'] = function(){
		try{animation('hide');}catch(err){}
		try{PutMessage('请求过程中发生错误,或返回格式错误无法解析');return false;}
		catch(err){}
	};
	SendOptions['complete'] = function(err)
	{
		try{
			var timer = setTimeout(function(){
				try{clearTimeout(timer);}catch(err){}
				try{animation("hide");}catch(err){}
			},500);
		}catch(err){}
	};
	/*********************************************************************
	*数据提交成功的返回信息处理
	**********************************************************************/
	SendOptions['success'] = function(rspJson)
	{
		var rspJson = rspJson || {};
		try{rspJson['type']=rspJson['type'] || 'define';	}catch(err){};
		try{rspJson['success']=rspJson['success'] || 'false';}catch(err){};
		try{rspJson['tips'] = rspJson['tips'] || '来自客户端的反馈发生未知错误,请重试!';}catch(err){};
		try{rspJson['text'] = rspJson['tips'] || '来自客户端的反馈发生未知错误,请重试!';}catch(err){};
		/******************************************************************************************
		*验证返回数据格式的合法性
		*******************************************************************************************/
		if(rspJson==undefined){PutMessage("获取请求数据返回格式失败！");return false;}
		else if(rspJson==null){PutMessage("获取请求数据返回格式失败！");return false;}
		else if(typeof(rspJson)!='object'){PutMessage("获取请求数据返回格式失败！");return false;}
		else if(rspJson['type']==undefined){PutMessage("返回请求返回数据类型失败！");return false;}
		else if(rspJson['type']==null){PutMessage("返回请求返回数据类型失败！");return false;}
		else if(rspJson['type']==''){PutMessage("返回请求返回数据类型失败！");return false;}
		else if(rspJson['tips']==undefined){PutMessage("获取请求数据处理结果失败！");return false;}
		else if(rspJson['tips']==null){PutMessage("获取请求数据处理结果失败！");return false;}
		else if(rspJson['tips']==''){PutMessage("获取请求数据处理结果失败！");return false;}
		else if(rspJson['success']==undefined){PutMessage("获取请求数据处理结果失败！");return false;}
		else if(rspJson['success']==null){PutMessage("获取请求数据处理结果失败！");return false;}
		else if(rspJson['success']!='true'){PutMessage(rspJson['tips']);return false;}
		/******************************************************************************************
		*返回数据处理结果
		*******************************************************************************************/
		if(rspJson!=undefined && typeof(rspJson)=='object' 
		&& callback!=undefined && callback!=null 
		&& typeof(callback)=='function' && rspJson['success']=='true')
		{
			try{callback(rspJson);}
			catch(err){}
		}else if(rspJson!=undefined && typeof(rspJson)=='object' 
		&& window.SaveBack!=undefined && window.SaveBack!=null 
		&& typeof(window.SaveBack)=='function' && rspJson['success']=='true')
		{
			try{window.SaveBack(rspJson);}
			catch(err){}
		}
		else if(rspJson['type']=='confirm'){doConfirm(rspJson);return false;}
		else if(rspJson['type']=='alert'){doMessage(rspJson);return false;}
		else if(rspJson['type']=='altRedirect'){altRedirect(rspJson);return false;}
		else if(rspJson['type']=='redirect'){doRedirect(rspJson);return false;}
		else if(rspJson['type']=='define'){doMessage(rspJson);return false;}
	};
	/*********************************************************************
	*发送数据请求信息
	**********************************************************************/
	if(SendOptions!=undefined && SendOptions!=null 
	&& typeof(SendOptions)=='object')
	{
		try{$(contianer).ajaxSubmit(SendOptions);}
		catch(err){}
	};
}
/****************************************************************************************
*处理服务器相应的数据信息
*****************************************************************************************/
var ShowResponse=function(rspJson)
{
	/*********************************************************************
	*输出错误处理提示信息
	***********************************************************************/
	var PutMessage = function(strMessage)
	{
		/*********************************************************************************
		*申明系统提示框提示数据信息
		**********************************************************************************/
		var rspJson = rspJson || {};
		try{rspJson['title'] = ('系统提示');}catch(err){}
		try{rspJson['text'] = (strMessage || '');}catch(err){}
		try{rspJson['type'] = 'error';}catch(err){}
		try{rspJson['confirmButtonClass'] = "btn-danger";}catch(err){}
		try{rspJson['confirmButtonText'] = '确定';}catch(err){}
		/*********************************************************************************
		*开始执行数据输出功能信息
		**********************************************************************************/
		if(window.swal!=undefined && window.swal!=null 
		&& typeof(window.swal)=='function')
		{
			try{window.swal(rspJson);}
			catch(err){}
		}
		else if(parent.swal!=undefined && parent.swal!=null 
		&& typeof(parent.swal)=='function')
		{
			try{parent.swal(rspJson);}
			catch(err){}
		}
		else if(top.swal!=undefined && top.swal!=null 
		&& typeof(top.swal)=='function')
		{
			try{top.swal(rspJson);}
			catch(err){}
		}
		else if(window.alert!=undefined && window.alert!=null 
		&& typeof(window.alert)=='function')
		{
			window.alert(rspJson['text']);
		}
	};
	/*********************************************************************
	*输出错误处理提示信息
	***********************************************************************/
	var doConfirm = function(rspJson)
	{
		var rspJson = rspJson || {};
		try{rspJson['title'] = ('系统提示');}catch(err){}
		try{rspJson['text'] = (rspJson['text'] || rspJson['tips']);}catch(err){}
		try{rspJson['type'] = (rspJson['success']=='true') ? 'success' : 'error';}
		catch(err){}
		try{rspJson['confirmButtonClass'] = "btn-danger";}catch(err){}
		try{rspJson['confirmButtonText'] = '确定';}catch(err){}
		try{rspJson['showCancelButton'] =  true;}catch(err){}
		try{rspJson['cancelButtonText'] = '取消';}catch(err){}
		try{rspJson['closeOnConfirm'] =  true;}catch(err){}
		try{rspJson['closeOnCancel'] =  true;}catch(err){}
		/*********************************************************************************
		*点击操作按钮事件信息
		**********************************************************************************/
		var doBack = function(confirmOK){
			if(confirmOK){
				if(rspJson['trueUrl']!=undefined 
				&& rspJson['trueUrl']!=null 
				&& rspJson['trueUrl']!='')
				{
					window.location=rspJson["trueUrl"];	
				}	
			}else{
				if(rspJson["falseUrl"]!=undefined 
				&& rspJson['trueUrl']!=null 
				&& rspJson["falseUrl"]!="")
				{
					
					window.location=rspJson["falseUrl"];
				}
				else{
					
					top.document.querySelector("#ContentFrame").window.location='?action=default'	
				;}
			}
		}
		/*********************************************************************************
		*开始执行数据输出功能信息
		**********************************************************************************/
		if(window.swal!=undefined && window.swal!=null 
		&& typeof(window.swal)=='function')
		{
			try{window.swal(rspJson,doBack);}
			catch(err){}
		}
		else if(parent.swal!=undefined && parent.swal!=null 
		&& typeof(parent.swal)=='function')
		{
			try{parent.swal(rspJson,doBack);}
			catch(err){}
		}
		else if(top.swal!=undefined && top.swal!=null 
		&& typeof(top.swal)=='function')
		{
			try{top.swal(rspJson,doBack);}
			catch(err){}
		}
		else if(window.confirm!=undefined && window.confirm!=null 
		&& typeof(window.confirm)=='function')
		{
			if(window.confirm(rspJson['text']))
			{
				doBack(true);	
			}
			else{doBack(false);	};
		}
	};
	/******************************************************************************************
	*弹出数据处理提示框信息
	*******************************************************************************************/
	var doMessage = function(rspJson)
	{
		/*********************************************************************************
		*申明系统提示框提示数据信息
		**********************************************************************************/
		var rspJson = rspJson || {};
		try{rspJson['title'] = ('系统提示');}catch(err){}
		try{rspJson['text'] = (rspJson['text'] || rspJson['tips']);}catch(err){}
		try{rspJson['type'] = (rspJson['success']=='true') ? 'success' : 'error';}
		catch(err){}
		try{rspJson['confirmButtonClass'] = "btn-danger";}catch(err){}
		try{rspJson['confirmButtonText'] = '确定';}catch(err){}
		/*********************************************************************************
		*开始执行数据输出功能信息
		**********************************************************************************/
		if(window.swal!=undefined && window.swal!=null 
		&& typeof(window.swal)=='function')
		{
			try{window.swal(rspJson);}
			catch(err){}
		}
		else if(parent.swal!=undefined && parent.swal!=null 
		&& typeof(parent.swal)=='function')
		{
			try{parent.swal(rspJson);}
			catch(err){}
		}
		else if(top.swal!=undefined && top.swal!=null 
		&& typeof(top.swal)=='function')
		{
			try{top.swal(rspJson);}
			catch(err){}
		}
		else if(window.alert!=undefined && window.alert!=null 
		&& typeof(window.alert)=='function')
		{
			window.alert(rspJson['text']);
		}
	};
	/******************************************************************************************
	*弹出确认框跳转到指定的界面
	*******************************************************************************************/
	var altRedirect = function(rspJson)
	{
		/*********************************************************************************
		*申明系统提示框提示数据信息
		**********************************************************************************/
		var rspJson = rspJson || {};
		try{rspJson['title'] = ('系统提示');}catch(err){}
		try{rspJson['text'] = (rspJson['text'] || rspJson['tips']);}catch(err){}
		try{rspJson['type'] = (rspJson['success']=='true') ? 'success' : 'error';}
		catch(err){}
		try{rspJson['confirmButtonClass'] = "btn-danger";}catch(err){}
		try{rspJson['confirmButtonText'] = '确定';}catch(err){}
		try{rspJson['showCancelButton'] = false;}catch(err){}
		try{rspJson['closeOnConfirm'] = true;}catch(err){}
		/*********************************************************************************
		*验证是否存在跳转的URL地址
		**********************************************************************************/
		if(rspJson['url']==undefined){PutMessage('获取跳转URL地址失败！');return false;}
		else if(rspJson['url']==null){PutMessage('获取跳转URL地址失败！');return false;}
		else if(rspJson['url']==''){PutMessage('获取跳转URL地址失败！');return false;}
		/*********************************************************************************
		*开始执行数据输出功能信息
		**********************************************************************************/
		if(window.swal!=undefined && window.swal!=null 
		&& typeof(window.swal)=='function')
		{
			try{window.swal(rspJson,function(isConfirm){window.location=rspJson['url'];});}
			catch(err){}
		}
		else if(parent.swal!=undefined && parent.swal!=null 
		&& typeof(parent.swal)=='function')
		{
			try{parent.swal(rspJson,function(isConfirm){window.location=rspJson['url'];});}
			catch(err){}
		}
		else if(top.swal!=undefined && top.swal!=null 
		&& typeof(top.swal)=='function')
		{
			try{top.swal(rspJson,function(isConfirm){window.location=rspJson['url'];});}
			catch(err){}
		}
		else if(window.alert!=undefined && window.alert!=null 
		&& typeof(window.alert)=='function')
		{
			window.alert(rspJson['text']);
			window.location=rspJson['url'];
		}
	};
	/******************************************************************************************
	*执行数据跳转功能
	*******************************************************************************************/
	var doRedirect = function(rspJson)
	{
		try{
			if(rspJson['url']==undefined){PutMessage('获取跳转URL地址失败！');return false;}
			else if(rspJson['url']==null){PutMessage('获取跳转URL地址失败！');return false;}
			else if(rspJson['url']==''){PutMessage('获取跳转URL地址失败！');return false;}
			else{window.location=rspJson['url'];}	
		}
		catch(err){}
	};
	/******************************************************************************************
	*将服务器端返回的JSON数据格式转换
	*******************************************************************************************/
	var rspJson = rspJson || {};
	try{rspJson['type']=rspJson['type'] || 'define';	}catch(err){};
	try{rspJson['success']=rspJson['success'] || 'false';}catch(err){};
	try{rspJson['tips'] = rspJson['tips'] || '来自客户端的反馈发生未知错误,请重试!';}catch(err){};
	try{rspJson['text'] = rspJson['tips'] || '来自客户端的反馈发生未知错误,请重试!';}catch(err){};
	/******************************************************************************************
	*验证返回数据格式的合法性
	*******************************************************************************************/
	if(rspJson==undefined){PutMessage("获取请求数据返回格式失败！");return false;}
	else if(rspJson==null){PutMessage("获取请求数据返回格式失败！");return false;}
	else if(typeof(rspJson)!='object'){PutMessage("获取请求数据返回格式失败！");return false;}
	else if(rspJson['type']==undefined){PutMessage("返回请求返回数据类型失败！");return false;}
	else if(rspJson['type']==null){PutMessage("返回请求返回数据类型失败！");return false;}
	else if(rspJson['type']==''){PutMessage("返回请求返回数据类型失败！");return false;}
	else if(rspJson['tips']==undefined){PutMessage("获取请求数据处理结果失败！");return false;}
	else if(rspJson['tips']==null){PutMessage("获取请求数据处理结果失败！");return false;}
	else if(rspJson['tips']==''){PutMessage("获取请求数据处理结果失败！");return false;}
	else if(rspJson['success']==undefined){PutMessage("获取请求数据处理结果失败！");return false;}
	else if(rspJson['success']==null){PutMessage("获取请求数据处理结果失败！");return false;}
	else if(rspJson['success']!='true'){PutMessage(rspJson['tips']);return false;}
	/******************************************************************************************
	*开始处理网站数据信息
	*******************************************************************************************/
	if(rspJson['type']=='confirm'){doConfirm(rspJson);return false;}
	else if(rspJson['type']=='alert'){doMessage(rspJson);return false;}
	else if(rspJson['type']=='altRedirect'){altRedirect(rspJson);return false;}
	else if(rspJson['type']=='redirect'){doRedirect(rspJson);return false;}	
	else if(rspJson['type']=='define'){
		if(options['back']!=undefined 
		&& options['back']!=null 
		&& typeof(options['back'])=='function')
		{
			options['back'](rspJson);
		};
	};
};
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
};
/*************************************************************************************
*ajax加载网页数据
**************************************************************************************/
;(function($){
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
				var timer = parseInt(options['timer']) || 6;
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
	
	/****************************************************************************
	*加载网页内容
	*****************************************************************************/
	$.fn.pager = function(options)
	{
		var $contianer = this;
		/**********************************************************************
		*生成一个内容装载框
		***********************************************************************/
		var $wapper = $("<div style=\"width:100%;clear:both;height:100%;\"></div>")[0];
		if($contianer!=undefined && $contianer!=null 
		&& $wapper!=undefined && $wapper!=null)
		{
			$($contianer).html($wapper);	
		}
		/**********************************************************************
		*申明延时动画的方法
		***********************************************************************/
		var WindowPager = null;
		var animation = function(closed)
		{
			if(closed!=undefined && closed!=null 
			&& closed=='show' && !WindowPager)
			{
				WindowPager = $("<div id=\"frmWindowPager\" style=\"position:fixed; width:145px; border-radius:3px; height:80px; background:rgba(0,0,0,0.85);left:calc((100% - 145px) / 2);left:-moz-calc((100% - 145px) / 2);left:-webkit-calc((100% - 145px) / 2);top:calc((100% - 80px) / 2);top:-moz-calc((100% - 80px) / 2);top:-webkit-calc((100% - 80px) / 2);\"><div style=\"background:url(data:image/gif;base64,R0lGODlhgACAAKIAAP///93d3bu7u5mZmQAA/wAAAAAAAAAAACH/C05FVFNDQVBFMi4wAwEAAAAh+QQFBQAEACwCAAIAfAB8AAAD/0i63P4wygYqmDjrzbtflvWNZGliYXiubKuloivPLlzReD7al+7/Eh5wSFQIi8hHYBkwHUmD6CD5YTJLz49USuVYraRsZ7vtar7XnQ1Kjpoz6LRHvGlz35O4nEPP2O94EnpNc2sef1OBGIOFMId/inB6jSmPdpGScR19EoiYmZobnBCIiZ95k6KGGp6ni4wvqxilrqBfqo6skLW2YBmjDa28r6Eosp27w8Rov8ekycqoqUHODrTRvXsQwArC2NLF29UM19/LtxO5yJd4Au4CK7DUNxPebG4e7+8n8iv2WmQ66BtoYpo/dvfacBjIkITBE9DGlMvAsOIIZjIUAixliv9ixYZVtLUos5GjwI8gzc3iCGghypQqrbFsme8lwZgLZtIcYfNmTJ34WPTUZw5oRxdD9w0z6iOpO15MgTh1BTTJUKos39jE+o/KS64IFVmsFfYT0aU7capdy7at27dw48qdS7eu3bt480I02vUbX2F/JxYNDImw4GiGE/P9qbhxVpWOI/eFKtlNZbWXuzlmG1mv58+gQ4seTbq06dOoU6vGQZJy0FNlMcV+czhQ7SQmYd8eMhPs5BxVdfcGEtV3buDBXQ+fURxx8oM6MT9P+Fh6dOrH2zavc13u9JXVJb520Vp8dvC76wXMuN5Sepm/1WtkEZHDefnzR9Qvsd9+/wi8+en3X0ntYVcSdAE+UN4zs7ln24CaLagghIxBaGF8kFGoIYV+Ybghh841GIyI5ICIFoklJsigihmimJOLEbLYIYwxSgigiZ+8l2KB+Ml4oo/w8dijjcrouCORKwIpnJIjMnkkksalNeR4fuBIm5UEYImhIlsGCeWNNJphpJdSTlkml1jWeOY6TnaRpppUctcmFW9mGSaZceYopH9zkjnjUe59iR5pdapWaGqHopboaYua1qije67GJ6CuJAAAIfkEBQUABAAsCgACAFcAMAAAA/9Iutz+ML5Ag7w46z0r5WAoSp43nihXVmnrdusrv+s332dt4Tyo9yOBUJD6oQBIQGs4RBlHySSKyczVTtHoidocPUNZaZAr9F5FYbGI3PWdQWn1mi36buLKFJvojsHjLnshdhl4L4IqbxqGh4gahBJ4eY1kiX6LgDN7fBmQEJI4jhieD4yhdJ2KkZk8oiSqEaatqBekDLKztBG2CqBACq4wJRi4PZu1sA2+v8C6EJexrBAD1AOBzsLE0g/V1UvYR9sN3eR6lTLi4+TlY1wz6Qzr8u1t6FkY8vNzZTxaGfn6mAkEGFDgL4LrDDJDyE4hEIbdHB6ESE1iD4oVLfLAqPETIsOODwmCDJlv5MSGJklaS6khAQAh+QQFBQAEACwfAAIAVwAwAAAD/0i63P5LSAGrvTjrNuf+YKh1nWieIumhbFupkivPBEzR+GnnfLj3ooFwwPqdAshAazhEGUXJJIrJ1MGOUamJ2jQ9QVltkCv0XqFh5IncBX01afGYnDqD40u2z76JK/N0bnxweC5sRB9vF34zh4gjg4uMjXobihWTlJUZlw9+fzSHlpGYhTminKSepqebF50NmTyor6qxrLO0L7YLn0ALuhCwCrJAjrUqkrjGrsIkGMW/BMEPJcphLgDaABjUKNEh29vdgTLLIOLpF80s5xrp8ORVONgi8PcZ8zlRJvf40tL8/QPYQ+BAgjgMxkPIQ6E6hgkdjoNIQ+JEijMsasNY0RQix4gKP+YIKXKkwJIFF6JMudFEAgAh+QQFBQAEACw8AAIAQgBCAAAD/kg0PPowykmrna3dzXvNmSeOFqiRaGoyaTuujitv8Gx/661HtSv8gt2jlwIChYtc0XjcEUnMpu4pikpv1I71astytkGh9wJGJk3QrXlcKa+VWjeSPZHP4Rtw+I2OW81DeBZ2fCB+UYCBfWRqiQp0CnqOj4J1jZOQkpOUIYx/m4oxg5cuAaYBO4Qop6c6pKusrDevIrG2rkwptrupXB67vKAbwMHCFcTFxhLIt8oUzLHOE9Cy0hHUrdbX2KjaENzey9Dh08jkz8Tnx83q66bt8PHy8/T19vf4+fr6AP3+/wADAjQmsKDBf6AOKjS4aaHDgZMeSgTQcKLDhBYPEswoA1BBAgAh+QQFBQAEACxOAAoAMABXAAAD7Ei6vPOjyUkrhdDqfXHm4OZ9YSmNpKmiqVqykbuysgvX5o2HcLxzup8oKLQQix0UcqhcVo5ORi+aHFEn02sDeuWqBGCBkbYLh5/NmnldxajX7LbPBK+PH7K6narfO/t+SIBwfINmUYaHf4lghYyOhlqJWgqDlAuAlwyBmpVnnaChoqOkpaanqKmqKgGtrq+wsbA1srW2ry63urasu764Jr/CAb3Du7nGt7TJsqvOz9DR0tPU1TIA2ACl2dyi3N/aneDf4uPklObj6OngWuzt7u/d8fLY9PXr9eFX+vv8+PnYlUsXiqC3c6PmUUgAACH5BAUFAAQALE4AHwAwAFcAAAPpSLrc/m7IAau9bU7MO9GgJ0ZgOI5leoqpumKt+1axPJO1dtO5vuM9yi8TlAyBvSMxqES2mo8cFFKb8kzWqzDL7Xq/4LB4TC6bz1yBes1uu9uzt3zOXtHv8xN+Dx/x/wJ6gHt2g3Rxhm9oi4yNjo+QkZKTCgGWAWaXmmOanZhgnp2goaJdpKGmp55cqqusrZuvsJays6mzn1m4uRAAvgAvuBW/v8GwvcTFxqfIycA3zA/OytCl0tPPO7HD2GLYvt7dYd/ZX99j5+Pi6tPh6+bvXuTuzujxXens9fr7YPn+7egRI9PPHrgpCQAAIfkEBQUABAAsPAA8AEIAQgAAA/lIutz+UI1Jq7026h2x/xUncmD5jehjrlnqSmz8vrE8u7V5z/m5/8CgcEgsGo/IpHLJbDqf0Kh0ShBYBdTXdZsdbb/Yrgb8FUfIYLMDTVYz2G13FV6Wz+lX+x0fdvPzdn9WeoJGAYcBN39EiIiKeEONjTt0kZKHQGyWl4mZdREAoQAcnJhBXBqioqSlT6qqG6WmTK+rsa1NtaGsuEu6o7yXubojsrTEIsa+yMm9SL8osp3PzM2cStDRykfZ2tfUtS/bRd3ewtzV5pLo4eLjQuUp70Hx8t9E9eqO5Oku5/ztdkxi90qPg3x2EMpR6IahGocPCxp8AGtigwQAIfkEBQUABAAsHwBOAFcAMAAAA/9Iutz+MMo36pg4682J/V0ojs1nXmSqSqe5vrDXunEdzq2ta3i+/5DeCUh0CGnF5BGULC4tTeUTFQVONYAs4CfoCkZPjFar83rBx8l4XDObSUL1Ott2d1U4yZwcs5/xSBB7dBMBhgEYfncrTBGDW4WHhomKUY+QEZKSE4qLRY8YmoeUfkmXoaKInJ2fgxmpqqulQKCvqRqsP7WooriVO7u8mhu5NacasMTFMMHCm8qzzM2RvdDRK9PUwxzLKdnaz9y/Kt8SyR3dIuXmtyHpHMcd5+jvWK4i8/TXHff47SLjQvQLkU+fG29rUhQ06IkEG4X/Rryp4mwUxSgLL/7IqFETB8eONT6ChCFy5ItqJomES6kgAQAh+QQFBQAEACwKAE4AVwAwAAAD/0i63A4QuEmrvTi3yLX/4MeNUmieITmibEuppCu3sDrfYG3jPKbHveDktxIaF8TOcZmMLI9NyBPanFKJp4A2IBx4B5lkdqvtfb8+HYpMxp3Pl1qLvXW/vWkli16/3dFxTi58ZRcChwIYf3hWBIRchoiHiotWj5AVkpIXi4xLjxiaiJR/T5ehoomcnZ+EGamqq6VGoK+pGqxCtaiiuJVBu7yaHrk4pxqwxMUzwcKbyrPMzZG90NGDrh/JH8t72dq3IN1jfCHb3L/e5ebh4ukmxyDn6O8g08jt7tf26ybz+m/W9GNXzUQ9fm1Q/APoSWAhhfkMAmpEbRhFKwsvCsmosRIHx444PoKcIXKkjIImjTzjkQAAIfkEBQUABAAsAgA8AEIAQgAAA/VIBNz+8KlJq72Yxs1d/uDVjVxogmQqnaylvkArT7A63/V47/m2/8CgcEgsGo/IpHLJbDqf0Kh0Sj0FroGqDMvVmrjgrDcTBo8v5fCZki6vCW33Oq4+0832O/at3+f7fICBdzsChgJGeoWHhkV0P4yMRG1BkYeOeECWl5hXQ5uNIAOjA1KgiKKko1CnqBmqqk+nIbCkTq20taVNs7m1vKAnurtLvb6wTMbHsUq4wrrFwSzDzcrLtknW16tI2tvERt6pv0fi48jh5h/U6Zs77EXSN/BE8jP09ZFA+PmhP/xvJgAMSGBgQINvEK5ReIZhQ3QEMTBLAAAh+QQFBQAEACwCAB8AMABXAAAD50i6DA4syklre87qTbHn4OaNYSmNqKmiqVqyrcvBsazRpH3jmC7yD98OCBF2iEXjBKmsAJsWHDQKmw571l8my+16v+CweEwum8+hgHrNbrvbtrd8znbR73MVfg838f8BeoB7doN0cYZvaIuMjY6PkJGSk2gClgJml5pjmp2YYJ6dX6GeXaShWaeoVqqlU62ir7CXqbOWrLafsrNctjIDwAMWvC7BwRWtNsbGFKc+y8fNsTrQ0dK3QtXAYtrCYd3eYN3c49/a5NVj5eLn5u3s6e7x8NDo9fbL+Mzy9/T5+tvUzdN3Zp+GBAAh+QQJBQAEACwCAAIAfAB8AAAD/0i63P4wykmrvTjrzbv/YCiOZGmeaKqubOu+cCzPdArcQK2TOL7/nl4PSMwIfcUk5YhUOh3M5nNKiOaoWCuWqt1Ou16l9RpOgsvEMdocXbOZ7nQ7DjzTaeq7zq6P5fszfIASAYUBIYKDDoaGIImKC4ySH3OQEJKYHZWWi5iZG0ecEZ6eHEOio6SfqCaqpaytrpOwJLKztCO2jLi1uoW8Ir6/wCHCxMG2x7muysukzb230M6H09bX2Nna29zd3t/g4cAC5OXm5+jn3Ons7eba7vHt2fL16tj2+QL0+vXw/e7WAUwnrqDBgwgTKlzIsKHDh2gGSBwAccHEixAvaqTYcFCjRoYeNyoM6REhyZIHT4o0qPIjy5YTTcKUmHImx5cwE85cmJPnSYckK66sSAAj0aNIkypdyrSp06dQo0qdSrWq1atYs2rdyrWr169gwxZJAAA7) center center no-repeat; position:absolute; width:100%;top:8px; height:40px; text-align:center; background-size:40px;\"></div><div style=\"width:100%; background:#000; height:24px; line-height:24px; text-align:center; color:#fff; position:absolute;left:0px;bottom:0px;\">Loading...</div></div>")[0];
			}
			/**********************************************************************
			*开始展示数据信息
			***********************************************************************/
			if(closed!=undefined && closed!=null 
			&& closed=='show' && WindowPager)
			{
				$(document.body).append(WindowPager);	
			}
			else if(closed!=undefined && closed!=null 
			&& closed=='hide' && WindowPager)
			{
				$(WindowPager).remove();	
			}
		};
		/**********************************************************************
		*加载网页内容信息
		***********************************************************************/
		var PagerCurrent = 1;
		/**********************************************************************
		*显示加载更多动画信息
		***********************************************************************/
		var frmEvent = null;
		var EventFunction = function()
		{
			try{
				frmEvent = $("<div style=\"width:100%;clear:both;color:#666;font-size:13px;;text-align:center;padding:8px 0px;\">加载更多<div>")[0];	
				if($contianer!=undefined && $contianer!=null 
				&& frmEvent!=undefined && frmEvent!=null)
				{
					$($contianer).append(frmEvent);	
				};
			}catch(err){}
			/**********************************************************************
			*定义点击事件信息
			***********************************************************************/
			$(frmEvent).click(function(){
				/*****************************************************************************
				*计算当前加载的索引信息
				******************************************************************************/
				try{PagerCurrent = PagerCurrent+1;}catch(err){}
				/*****************************************************************************
				*计算当前应该请求的Url信息
				******************************************************************************/
				try
				{
					var params = options['url'] || document.url;
					if(params!=undefined && params!="" 
					&& params.toLowerCase().indexOf('page=')!=-1)
					{
						params = params.replace(/page=(\d+)/,"page="+PagerCurrent);
						
					}else if(params!=undefined && params!="")
					{
						if(params.indexOf("?")!=-1){params=params+"&page=2";}
						else{params=params+"?page=2";}	
					}
					options['url'] = params;
				}catch(err){}
				/*****************************************************************************
				*重新加载分页内容
				******************************************************************************/
				if(options['url']!=undefined && options['url']!="")
				{
					try{SendResponse();}catch(err){}	
				}					   
			});
		};
		
		/**********************************************************************
		*加载网页内容信息
		***********************************************************************/
		var SendResponse = function()
		{
			if(options!=undefined && options!=null && typeof(options)=='object' 
			&& options['url']!=undefined && options['url']!=null && options['url']!="")
			{
				animation('show');
				/**********************************************************************
				*申明pager集合信息
				***********************************************************************/
				var SendOptions = {};
				/**********************************************************************
				*请求的url
				***********************************************************************/
				SendOptions["url"] = options['url'];
				/**********************************************************************
				*定义请求方式
				***********************************************************************/
				SendOptions["type"] = options['type'] || "get";
				/**********************************************************************
				*定义数据请求类型
				***********************************************************************/
				if( options['dataType']!=undefined && options['dataType']!="")
				{SendOptions["dataType"] = options['dataType'];}
				/**********************************************************************
				*定义是否同步处理信息
				***********************************************************************/
				SendOptions["async"] = true;
				/**********************************************************************
				*数据处理成功事件信息
				***********************************************************************/
				SendOptions["success"] = function(strResponse)
				{
					if(options['dateType']!=undefined && options['dateType']=='json' 
					&& strResponse!=undefined && typeof(strResponse)=='object' 
					&& strResponse['url']!=undefined && strResponse['url']!=null)
					{
						window.location = strResponse['url'];
					}
					else if(options['dateType']!=undefined && options['dateType']=='json' 
					&& strResponse!=undefined && typeof(strResponse)=='object' 
					&& options['back']!=undefined && options['back']!=null 
					&& typeof(options['back'])=='function')
					{
						try{options['back'](strResponse);}catch(err){}
					}
					else if(options['back']!=undefined && options['back']!=null 
					&& typeof(options['back'])=='function' 
					&& strResponse!=undefined && strResponse!="")
					{
						try{options['back'](strResponse);}catch(err){}	
					}
					else if(options!=undefined && typeof(options)=='object' 
					&& strResponse!=undefined && strResponse!="")
					{
						try{
							if(options!=undefined && options!=null 
							&& options['ipager']!=undefined && options['ipager']=='true' 
							&& $wapper!=undefined && $wapper!=null)
							{
								$($wapper).append(strResponse);	
							}
							else if(options!=undefined && options!=null 
							&& $wapper!=undefined && $wapper!=null)
							{
								$($wapper).html(strResponse);
								/****************************************************************
								* 定义分页事件
								*****************************************************************/
								try{
									$($wapper).find("#fenyePageBar").find("a").click(function(){
										try{
											var href = $(this).attr("href") || "";
											if(href!=undefined && href!=null 
											&& href!="" && href.indexOf('javascript')==-1)
											{
												options['url'] = href;
												try{SendResponse();}catch(err){}
												return false;
											}
											else{return false;}
										}catch(err){return false;}
										
									});
								}catch(err){}
								/****************************************************************
								* 定义分页事件
								*****************************************************************/
								try{
									$($wapper).find("#frm-select-chg").change(function(){
										try{
											var href = $(this).val();
											if(href!=undefined && href!=null 
											&& href!="" && href.indexOf('javascript')==-1)
											{
												options['url'] = href;
												try{SendResponse();}catch(err){}
											}
										}catch(err){}
									});
								}catch(err){}
							}
						}catch(err){};
						/**********************************************************************
						*定义数据加载完成的事件
						***********************************************************************/
						if(options!=undefined && options!=null 
						&& options['complate']!=undefined && options['complate']!=null 
						&& typeof(options['complate'])=='function')
						{
							options['complate']($wapper);	
						}
					}
					else if(options!=undefined && typeof(options)=='object' 
					&& options['ipager']!=undefined && options['ipager']=='true'
					&& strResponse!=undefined && (strResponse=="false" || strResponse==""))
					{
						try{
							$(frmEvent).html('没有更多数据了');
							$(frmEvent).unbind();
							var timer = 5;
							var interval = setInterval(function(){
								if(timer>=0){timer=timer-1;}
								else{
									$(frmEvent).hide();
									clearInterval(interval);
								}
							},1000);
						}catch(err){}
					}
					/**********************************************************************
					*关闭动画信息
					***********************************************************************/
					try{animation('hide');}catch(err){}
				};
				/**********************************************************************
				*定义错误返回事件信息
				***********************************************************************/
				SendOptions["error"] = function(err)
				{
					try{
						if(err!=undefined && typeof(err)=='object' 
						&& err['statusText']!=undefined && err['statusText']!='')
						{
							$('#frm-windowError').tips({
								"success":"false",
								"tips":('发生错误:'+err["statusText"])
							});
						};
					}catch(err){};
					/**********************************************************************
					*关闭动画信息
					***********************************************************************/
					try{animation('hide');}catch(err){}
				};
				/**********************************************************************
				*开始处理数据信息
				***********************************************************************/
				if(SendOptions!=undefined && SendOptions!=null && typeof(SendOptions)=='object' 
				&& SendOptions['url']!=undefined && SendOptions['url']!="")
				{
					try{$.ajax(SendOptions);}
					catch(err){}
				}
			}	
		};
		/******************************************************************************
		*创建定义内容信息
		*******************************************************************************/
		try{SendResponse();}catch(err){}
		/******************************************************************************
		*定义自动分页信息
		*******************************************************************************/
		try
		{
			if(options!=undefined && options!=null && typeof(options)=='object'
			&& options['ipager']!=undefined && options['ipager']=='true')
			{
				EventFunction();	
			}
		}catch(err){}	
	};
	
	/**********************************************************************
	*加载网页内容信息
	***********************************************************************/
	var GetResponse = function(options,back)
	{
		if(options!=undefined && options!=null && typeof(options)=='object' 
		&& options['url']!=undefined && options['url']!=null && options['url']!="")
		{
			//animation('show');
			/**********************************************************************
			*申明pager集合信息
			***********************************************************************/
			var SendOptions = {};
			/**********************************************************************
			*请求的url
			***********************************************************************/
			SendOptions["url"] = options['url'];
			/**********************************************************************
			*定义请求方式
			***********************************************************************/
			SendOptions["type"] = options['type'] || "get";
			/**********************************************************************
			*定义数据请求类型
			***********************************************************************/
			SendOptions["dataType"] = options['dataType'] || "json";
			/**********************************************************************
			*定义是否同步处理信息
			***********************************************************************/
			SendOptions["async"] = true;
			/**********************************************************************
			*数据处理成功事件信息
			***********************************************************************/
			SendOptions["success"] = function(strResponse)
			{
				if(options['dateType']!=undefined && options['dateType']=='json' 
				&& strResponse!=undefined && typeof(strResponse)=='object' 
				&& strResponse['url']!=undefined && strResponse['url']!=null)
				{
					window.location = strResponse['url'];
				}
				else if(options['dateType']!=undefined && options['dateType']=='json' 
				&& strResponse!=undefined && typeof(strResponse)=='object' 
				&& options['back']!=undefined && options['back']!=null 
				&& typeof(options['back'])=='function')
				{
					try{options['back'](strResponse);}catch(err){}
				}
				else if(options['back']!=undefined && options['back']!=null 
				&& typeof(options['back'])=='function' && strResponse!=undefined && strResponse!="")
				{
					try{options['back'](strResponse);}catch(err){}	
				};
				/**********************************************************************
				*关闭动画信息
				***********************************************************************/
				try{animation('hide');}catch(err){}
			};
			/**********************************************************************
			*定义错误返回事件信息
			***********************************************************************/
			SendOptions["error"] = function(err)
			{
				try{
					if(err!=undefined && typeof(err)=='object' 
					&& err['statusText']!=undefined && err['statusText']!='')
					{
						$('#frm-windowError').tips({
							"success":"false",
							"tips":('发生错误:'+err["statusText"])
						});
					};
				}catch(err){};
				/**********************************************************************
				*关闭动画信息
				***********************************************************************/
				//try{animation('hide');}catch(err){}
			};
			/**********************************************************************
			*开始处理数据信息
			***********************************************************************/
			if(SendOptions!=undefined && SendOptions!=null && typeof(SendOptions)=='object' 
			&& SendOptions['url']!=undefined && SendOptions['url']!=null && SendOptions['url']!="" 
			&& jQuery!=undefined && jQuery!=null && typeof(jQuery)=='function' 
			&& jQuery.ajax!=undefined && jQuery.ajax!=null && typeof(jQuery.ajax)=='function')
			{
				try{jQuery.ajax(SendOptions);}catch(err){}
			}
		};
	};
	
	/********************************************************************************************
	*☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
	* send message apis
	*☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆
	*********************************************************************************************/
	$.fn.captcha = function(options)
	{
		/**********************************************************************
		*获取请求对象信息
		***********************************************************************/
		var getCaptcha = function(callback)
		{
			var SendOptions = {};
			/**********************************************************************
			*设置请求类型
			***********************************************************************/
			SendOptions["url"] = options['url'];
			/**********************************************************************
			*处理请求方式
			***********************************************************************/
			SendOptions["type"] = options['type'] || "get";
			/**********************************************************************
			*处理请求格式
			***********************************************************************/
			SendOptions["dataType"] = "json";
			/**********************************************************************
			*设置为异步请求
			***********************************************************************/
			SendOptions["async"] = true;
			/**********************************************************************
			*处理失败请求
			***********************************************************************/
			SendOptions["error"] = function(err)
			{
				try{ShowMessager({"text":'获取短信验证码失败,请重试!'});}
				catch(err){}
			};
			SendOptions["success"] = function(rspJson)
			{
				try{
					if(rspJson!=undefined && rspJson!=null && typeof(rspJson)=='object'
					&& rspJson['success']!=undefined && rspJson['success']=='true' 
					&& callback!=undefined && callback!=null && typeof(callback)=='function'){
						try{callback();}catch(err){}
					}
					else if(rspJson!=undefined && rspJson!=null && typeof(rspJson)=='object'
					&& rspJson['success']!=undefined && rspJson['success']!=null && rspJson['success']!='true' 
					&& rspJson['tips']!=undefined && rspJson['tips']!=null && rspJson['tips']!="")
					{
						try{ShowMessager({"text":(rspJson['tips'])});}
						catch(err){}
					}
				}catch(err){}
			};
			/**********************************************************************
			*开始发送请求数据
			***********************************************************************/
			if(SendOptions!=undefined && SendOptions!=null && typeof(SendOptions)=='object' 
			&& SendOptions['url']!=undefined && SendOptions['url']!=null && SendOptions['url']!="" 
			&& jQuery!=undefined && jQuery!=null && typeof(jQuery)=='function' 
			&& jQuery.ajax!=undefined && jQuery.ajax!=null && typeof(jQuery.ajax)=='function')
			{
				try{jQuery.ajax(SendOptions);}
				catch(err){}
			}
		};
		/**********************************************************************
		*获取节点对象
		***********************************************************************/
		var contianer = this;
		var Retryname = $(contianer).val();
		if(contianer!=undefined && contianer!=null){
			getCaptcha(function(){
				try{
					try{$(contianer).attr('disabled','disabled');}catch(err){}
					var sTimer = 60;
					var Interval = setInterval(function(){
						if(sTimer>=1){
							sTimer=sTimer-1;
							try{$(contianer).val('重新获取 '+sTimer+'s');}
							catch(err){}
						}else{
							try{$(contianer).val(Retryname);}catch(err){}
							try{clearInterval(Interval);}catch(err){}
							try{$(contianer).removeAttr('disabled');}
							catch(err){}
						};
					},1000);
				}catch(err){}
			});
		}
	};
	
	/******************************************************************************
	*生成网页选择扩展器
	*******************************************************************************/
	$.fn.contianer = function(options)
	{
		var $this = null;
		try{
			if(this!=undefined && this!=null 
			&& this[0]!=undefined && this[0]!=null)
			{
				$this = this[0];
			}
			else
			{
				if($("div[operate=\"frmContianer\"]")[0]!=undefined 
				&& $("div[operate=\"frmContianer\"]")[0]!=null)
				{
					$this = $("div[operate=\"frmContianer\"]")[0];	
				}
				else
				{
					$this = document.createElement('div');
					if(document!=undefined && document!=null 
					&& document.body!=undefined && document.body!=null)
					{
						$(document.body).append($this);
					}
					if($this!=undefined && $this!=null)
					{
						$($this).attr('operate','frmContianer');
						$($this).attr('zoom','false');
						$($this).addClass('frmContianer');
					}
				}
			}
		}catch(err){}
		/*****************************************************************
		*设置元素operate的值
		******************************************************************/
		if($this!=undefined && $this!=null){
			try{
				var operateText = $($this).attr("operate") || "";
				if(operateText==undefined || operateText==null 
				|| operateText=="" || operateText!="frmContianer")
				{
					$($this).attr("operate","frmContianer");	
				}
			}catch(err){}
		}
		/*****************************************************************
		*获取窗口标题信息
		******************************************************************/
		if($this!=undefined && $this!=null 
		&& options!=undefined && typeof(options)=='object')
		{
			try{
				var title = $($this).attr('title');
				if(title!=undefined && title!="" 
				&& (options['title']==undefined || options['title']==''))
				{
					options['title'] = title;
				}
			}catch(err){}
		}
		/*****************************************************************
		*显示窗口标题信息
		******************************************************************/
		if($this!=undefined && $this!=null)
		{
			try{
				var frmTitle = $($this).find("div[operate=\"frmTitle\"]")[0];
				if(frmTitle==undefined || frmTitle==null)
				{ 
					frmTitle = document.createElement('div');
					$(frmTitle).attr('operate','frmTitle');
					$($this).append(frmTitle);
				}
			}catch(err){}
			try{
				if(options!=undefined && options!=null && typeof(options)=='object' 
				&& options['title']!=undefined && options['title']!="")
				{$(frmTitle).html(options['title']);	}
			}catch(err){}
		}
		/*****************************************************************
		*显示关闭窗口信息
		******************************************************************/
		if($this!=undefined && $this!=null && options!=undefined 
		&& options!=null && typeof(options)=='object' && options['isclose']!="false")
		{
			try{
				var frmClose = $($this).find("div[operate=\"close\"]")[0];
				if(frmClose==undefined && frmClose==null){
					frmClose = document.createElement('div');
					$(frmClose).attr('operate','close');
					/*****************************************************************
					*设置关闭按钮事件
					******************************************************************/
					try{
						$(frmClose).click(function(){
							/*****************************************************************
							*窗体关闭之前发生事件
							******************************************************************/
							var closeOK = false;
							try{
								if(options!=undefined && options!=null && typeof(options)=='object' 
								&& options['onclose']!=undefined && options['onclose']!=null 
								&& typeof(options['onclose'])=='function')
								{
									try{
										options['onclose'](function(isClosed){
											closeOK = isClosed; 
											if(isClosed){$($this).hide('slow');}
										});
									}catch(err){}
								}else{
									closeOK = false;
									$($this).hide('slow');
								}
							}catch(err){}
							/*****************************************************************
							*窗体关闭事件
							******************************************************************/
							try{
								if(options!=undefined && options!=null && typeof(options)=='object' 
								&& options['closed']!=undefined && options['closed']!=null 
								&& typeof(options['closed'])=='function' && closeOK)
								{
									options['closed']();
								}
							}catch(err){}
							return false;
						});
					}catch(err){}
					$($this).append(frmClose);
				}
			}catch(err){};
		};
		/*****************************************************************
		*设置窗口放大缩小信息
		******************************************************************/
		if($this!=undefined && $this!=null && options!=undefined 
		&& options!=null && typeof(options)=='object' && options['iszoom']!="false")
		{
			try{
				var frmZoom = $($this).find("div[operate=\"zoom\"]")[0];
				if(frmZoom==undefined || frmZoom==null){
					frmZoom = document.createElement('div');
					$(frmZoom).attr('operate','zoom');
					$(frmZoom).bind("click",function(){
						var zoomIndex = $($this).attr("zoom") || "false";
						if(zoomIndex!=undefined && zoomIndex!="" && zoomIndex=="false")
						{
							$($this).attr("zoom","true");
						}else{
							$($this).attr("zoom","false");	
						};
					});
					$($this).append(frmZoom);
				}
				/*****************************************************************
				*重新还原窗口
				******************************************************************/
				if(frmZoom!=undefined && frmZoom!=null)
				{
					$($this).attr("zoom","false");	
				}
			}catch(err){}
		};
		/*****************************************************************
		*加载iframe框架内容
		******************************************************************/
		try{
			var frameContianer = null;
			if($this!=undefined && $this!=null)
			{
				try{
					frameContianer = $($this).find("div[operate=\"frameContianer\"]")[0];
					if(frameContianer==undefined || frameContianer==null){
						frameContianer = document.createElement('div');
						$(frameContianer).attr('operate','frameContianer');
						$($this).append(frameContianer);	
					}
				}catch(err){}
			}
		}catch(err){}
		/*****************************************************************
		*开始加载指定的框架内容信息
		******************************************************************/
		if($this!=undefined && $this!=null 
		&& frameContianer!=undefined && frameContianer!=null)
		{
			if(options!=undefined && options!=null 
			&& typeof(options)=='string' && options=='close')
			{
				try{$($this).hide('slow');}catch(err){}
			}
			else if(options!=undefined && options!=null 
			&& typeof(options)=='string' && options=='zoom')
			{
				try{
					var zoomIndex = $($this).attr("zoom") || "false";
					if(zoomIndex!=undefined && zoomIndex!="" && zoomIndex=="false")
					{
						$($this).attr("zoom","true");
					}else{
						$($this).attr("zoom","false");	
					};
				}catch(err){}
			}
			else if(options!=undefined && options!=null && typeof(options)=='object'
			&& options['zoom']!=undefined && options['zoom']!="" 
			&& options['width']!=undefined && options['width']!="")
			{
				try{
					var sWidth = parseFloat(options['width']) || 0;
					$($this).css({"width":sWidth});
				}catch(err){}
			}
			else
			{
				var options = options || {};
				try
				{
					/*****************************************************************
					*设置窗体名称信息
					******************************************************************/
					if(options['name']==undefined || options['name']=='')
					{
						options['name'] = 'define';
					}
					/*****************************************************************
					*设置网页标题信息
					******************************************************************/
					if(options['title']!=undefined && options['title']!='')
					{
						$($this).attr('title',options['title']);	
					}
					/*****************************************************************
					*加载网页宽度
					******************************************************************/
					if(options['width']!=undefined && options['width']!="" && options['width']!="0")
					{
						$($this).css({"width":options['width']});
					}
				}catch(err){}
				/*****************************************************************
				*是否全屏加载内容
				******************************************************************/
				if(options['zoom']!=undefined && options['zoom']!="")
				{
					try{
						if(options['zoom']!="true")
						{
							$($this).attr("zoom","false");
						}else
						{
							$($this).attr("zoom","true");	
						};
					}catch(err){}
				}
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
				try
				{
					$(frameContianer).find("iframe[operate=\"frame\"]").hide();
				}
				catch(err){}
				/*****************************************************************
				*判断指定的框架是否存在,否则执行创建
				******************************************************************/
				try{
					var Frame = $(frameContianer).find("iframe[name=\""+options['name']+"\"]")[0];
					if(Frame!=undefined && Frame!=null)
					{
						if(options!=undefined && options!=null 
						&& options['reload']!=undefined && options['reload']=='true'){
							if(options['url']!=undefined && options['url']!='')
							{
								$(Frame).attr({'src':options['url']});
							}
						}
					}
					else{
						try{
							Frame = document.createElement('iframe');
							$(Frame).attr({'name':options['name'],'frameborder':'0','scrolling':'auto'});
							$(Frame).attr({'operate':'frame'});
							if(options['url']!=undefined && options['url']!='')
							{$(Frame).attr({'src':options['url']});}
							$(frameContianer).append(Frame);	
						}catch(err){}
					};
				}catch(err){}
				
				/*****************************************************************
				*网页加载完成的回调函数
				******************************************************************/
				if(Frame!=undefined && Frame!=null){
					try{
						if(options!=undefined && options!=null && typeof(options)=='object'
						&& options['onload']!=undefined && options['onload']!=null 
						&& typeof(options['onload'])=='function')
						{
							options['onload']();
						}
					}catch(err){}
				}
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
								/*****************************************************************
								*网页加载完成的回调函数
								******************************************************************/
								try{
									if(options!=undefined && options!=null && typeof(options)=='object'
									&& options['complete']!=undefined && options['complete']!=null 
									&& typeof(options['complete'])=='function')
									{
										options['complete']();
									}
								}catch(err){}
								/*****************************************************************
								*开始关闭网页窗体
								******************************************************************/
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
		}
	};
})(jQuery);
























