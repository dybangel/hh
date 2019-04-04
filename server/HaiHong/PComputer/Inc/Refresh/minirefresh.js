/*!
 * minirefresh v2.0.2
 * (c) 2017-2018 dailc
 * Released under the MIT License.
 * https://github.com/minirefresh/minirefresh
 */
(function (global, factory) {
	typeof exports === 'object' && typeof module !== 'undefined' ? module.exports = factory() :
	typeof define === 'function' && define.amd ? define(factory) :
	(global.MiniRefreshTools = factory());
}(this, (function () { 'use strict';

function getNow() {
    return window.performance && (window.performance.now ? window.performance.now() + window.performance.timing.navigationStart : +new Date());
}

var noop = function noop() {};

function isArray(object) {
    if (Array.isArray) {
        return Array.isArray(object);
    }

    return object instanceof Array;
}

function isObject(object) {
    var classType = Object.prototype.toString.call(object).match(/^\[object\s(.*)\]$/)[1];

    return classType !== 'String' && classType !== 'Number' && classType !== 'Boolean' && classType !== 'Undefined' && classType !== 'Null';
}

function isWindow(object) {
    return object && object === window;
}

function isPlainObject(obj) {
    return isObject(obj) && !isWindow(obj)
    // 如果不是普通的object,Object.prototype需要通过链回溯才能得到
    && Object.getPrototypeOf(obj) === Object.prototype;
}

function extend() {
    var _arguments = arguments;

    var len = arguments.length;
    var target = (arguments.length <= 0 ? undefined : arguments[0]) || {};
    var sourceIndex = 1;
    var isDeep = false;

    if (typeof target === 'boolean') {
        // 深赋值或false
        isDeep = target;
        target = (arguments.length <= sourceIndex ? undefined : arguments[sourceIndex]) || {};
        sourceIndex++;
    }

    if (!isObject(target)) {
        // 确保拓展的一定是object
        target = {};
    }

    var _loop = function _loop() {
        // source的拓展
        var source = _arguments.length <= sourceIndex ? undefined : _arguments[sourceIndex];

        if (source && isObject(source)) {
            // for-of打包过大
            Object.keys(source).forEach(function (name) {
                var src = target[name];
                var copy = source[name];
                var copyIsPlainObject = isPlainObject(copy);
                var copyIsArray = isArray(copy);
                var clone = void 0;

                if (target === copy) {
                    // 防止环形引用
                    return;
                }

                if (isDeep && copy && (copyIsArray || copyIsPlainObject)) {
                    // 这里必须用isPlainObject,只有同样是普通的object才会复制继承
                    // 如果是FormData之流的，会走后面的覆盖路线
                    if (copyIsArray) {
                        copyIsArray = false;
                        clone = src && isArray(src) ? src : [];
                    } else {
                        clone = src && isPlainObject(src) ? src : {};
                    }

                    target[name] = extend(isDeep, clone, copy);
                } else if (copy !== undefined) {
                    // 如果非深赋值
                    // 或者不是普通的object，直接覆盖，例如FormData之类的也会覆盖
                    target[name] = copy;
                }
            });
        }
    };

    for (; sourceIndex < len; sourceIndex++) {
        _loop();
    }

    return target;
}

/**
 * 选择这段代码用到的太多了，因此抽取封装出来
 * @param {Object} element dom元素或者selector
 * @return {HTMLElement} 返回选择的Dom对象，无果没有符合要求的，则返回null
 */
function selector(element) {
    var target = element;

    if (typeof target === 'string') {
        target = document.querySelector(target);
    }

    return target;
}

/**
 * 获取DOM的可视区高度，兼容PC上的body高度获取
 * 因为在通过body获取时，在PC上会有CSS1Compat形式，所以需要兼容
 * @param {HTMLElement} dom 需要获取可视区高度的dom,对body对象有特殊的兼容方案
 * @return {Number} 返回最终的高度
 */
function getClientHeightByDom(dom) {
    var height = dom.clientHeight;

    if (dom === document.body && document.compatMode === 'CSS1Compat') {
        // PC上body的可视区的特殊处理
        height = document.documentElement.clientHeight;
    }

    return height;
}

/**
 * 设置一个Util对象下的命名空间
 * @param {Object} parent 需要绑定到哪一个对象上
 * @param {String} namespace 需要绑定的命名空间名
 * @param {Object} target 需要绑定的目标对象
 * @return {Object} 返回最终的对象
 */
function namespace(parent, namespaceStr, target) {
    if (!namespaceStr) {
        return parent;
    }

    var namespaceArr = namespaceStr.split('.');
    var len = namespaceArr.length;
    var res = parent;

    for (var i = 0; i < len - 1; i += 1) {
        var tmp = namespaceArr[i];

        // 不存在的话要重新创建对象
        res[tmp] = res[tmp] || {};
        // parent要向下一级
        res = res[tmp];
    }
    res[namespaceArr[len - 1]] = target;

    return target;
}

var lang = Object.freeze({
	getNow: getNow,
	noop: noop,
	isArray: isArray,
	isObject: isObject,
	isWindow: isWindow,
	isPlainObject: isPlainObject,
	extend: extend,
	selector: selector,
	getClientHeightByDom: getClientHeightByDom,
	namespace: namespace
});

/**
 * 加入系统判断功能
 */
function osMixin(hybrid) {
    var hybridJs = hybrid;
    var detect = function detect(ua) {
        this.os = {};

        var android = ua.match(/(Android);?[\s/]+([\d.]+)?/);

        if (android) {
            this.os.android = true;
            this.os.version = android[2];
            this.os.isBadAndroid = !/Chrome\/\d/.test(window.navigator.appVersion);
        }

        var iphone = ua.match(/(iPhone\sOS)\s([\d_]+)/);

        if (iphone) {
            this.os.ios = true;
            this.os.iphone = true;
            this.os.version = iphone[2].replace(/_/g, '.');
        }

        var ipad = ua.match(/(iPad).*OS\s([\d_]+)/);

        if (ipad) {
            this.os.ios = true;
            this.os.ipad = true;
            this.os.version = ipad[2].replace(/_/g, '.');
        }

        // quickhybrid的容器
        var quick = ua.match(/QuickHybrid/i);

        if (quick) {
            this.os.quick = true;
        }

        // epoint的容器
        var ejs = ua.match(/EpointEJS/i);

        if (ejs) {
            this.os.ejs = true;
        }

        var dd = ua.match(/DingTalk/i);

        if (dd) {
            this.os.dd = true;
        }

        // 如果ejs和钉钉以及quick都不是，则默认为h5
        if (!ejs && !dd && !quick) {
            this.os.h5 = true;
        }
    };

    detect.call(hybridJs, navigator.userAgent);
}

var DEFAULT_INTERVAL = 1000 / 60;

// 立即执行
var requestAnimationFrame = function () {
    return window.requestAnimationFrame || window.webkitRequestAnimationFrame || window.mozRequestAnimationFrame || window.oRequestAnimationFrame
    // if all else fails, use setTimeout
    || function requestAnimationFrameTimeOut(callback) {
        // make interval as precise as possible.
        return window.setTimeout(callback, (callback.interval || DEFAULT_INTERVAL) / 2);
    };
}();

var _createClass$1 = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _classCallCheck$1(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

/**
 * 一些事件
 */
var EVENT_INIT = 'initScroll';
var EVENT_SCROLL = 'scroll';
var EVENT_PULL = 'pull';
var EVENT_UP_LOADING = 'upLoading';
var EVENT_RESET_UP_LOADING = 'resetUpLoading';
var EVENT_DOWN_LOADING = 'downLoading';
var EVENT_CANCEL_LOADING = 'cancelLoading';

/**
 * 一些hook
 * hook是指挥它会影响逻辑
 */
var HOOK_BEFORE_DOWN_LOADING = 'beforeDownLoading';

var PER_SECOND = 1000 / 60;

/**
 * 滑动操作相关类
 * 把一些滑动滚动逻辑单独剥离出来
 * 确保Core中只有纯粹的API定义
 */

var Scroll = function () {
    /**
     * 传入minirefresh对象，因为内部一些配置项依赖于minirefresh
     * @param {Object} options 配置信息
     * @constructor
     */
    function Scroll(minirefresh) {
        _classCallCheck$1(this, Scroll);

        this.contentWrap = minirefresh.contentWrap;
        this.scrollWrap = minirefresh.scrollWrap;
        this.options = minirefresh.options;
        this.os = minirefresh.os;
        // 默认没有事件，需要主动绑定
        this.events = {};
        // 默认没有hook
        this.hooks = {};

        // 使用了scrollto后加锁，防止重复
        this.isScrollTo = false;
        // 上拉和下拉的状态
        this.upLoading = false;
        this.downLoading = false;
        // 默认up是没有finish的
        this.isFinishUp = false;

        this._init();
    }

    _createClass$1(Scroll, [{
        key: '_init',
        value: function _init() {
            var _this = this;

            this._initPullDown();
            this._initPullUp();

            setTimeout(function () {
                if (_this.options.down && _this.options.down.isAuto && !_this.options.down.isLock) {
                    // 满足自动下拉,需要判断是否需要动画（仅仅是首次）
                    if (_this.options.down.isAllowAutoLoading) {
                        _this.triggerDownLoading();
                    } else {
                        _this.events[EVENT_DOWN_LOADING] && _this.events[EVENT_DOWN_LOADING](true);
                    }
                } else if (_this.options.up && _this.options.up.isAuto && !_this.options.up.isLock) {
                    // 满足上拉，上拉的配置由配置项决定（每一次）
                    _this.triggerUpLoading();
                }

                _this.events[EVENT_INIT] && _this.events[EVENT_INIT]();
            });
        }
    }, {
        key: 'refreshOptions',
        value: function refreshOptions(options) {
            this.options = options;
        }

        /**
         * ContentWrap的translate动画，用于下拉刷新时进行transform动画
         * @param {Number} y 移动的高度
         * @param {Number} duration 过渡时间
         */

    }, {
        key: 'translateContentWrap',
        value: function translateContentWrap(y, duration) {
            var translateY = y || 0;
            var translateDuration = duration || 0;

            // 改变downHight， 这个参数关乎逻辑
            this.downHight = translateY;

            if (!this.options.down.isScrollCssTranslate) {
                // 只有允许动画时才会scroll也translate,否则只会改变downHeight
                return;
            }

            // 改变wrap的位置（css动画）
            var wrap = this.contentWrap;

            wrap.style.webkitTransitionDuration = translateDuration + 'ms';
            wrap.style.transitionDuration = translateDuration + 'ms';
            wrap.style.webkitTransform = 'translate(0px, ' + translateY + 'px) translateZ(0px)';
            wrap.style.transform = 'translate(0px, ' + translateY + 'px) translateZ(0px)';
        }
    }, {
        key: '_scrollWrapAnimation',
        value: function _scrollWrapAnimation() {
            this.scrollWrap.webkitTransitionTimingFunction = 'cubic-bezier(0.1, 0.57, 0.1, 1)';
            this.scrollWrap.transitionTimingFunction = 'cubic-bezier(0.1, 0.57, 0.1, 1)';
        }
    }, {
        key: '_initPullDown',
        value: function _initPullDown() {
            var _this2 = this;

            // 考虑到options可以更新，所以不能被缓存，而是应该在回调中直接获取
            var scrollWrap = this.scrollWrap;
            var docClientHeight = document.documentElement.clientHeight;

            this._scrollWrapAnimation();

            // 触摸开始
            var touchstartEvent = function touchstartEvent(e) {
                if (_this2.isScrollTo) {
                    // 如果执行滑动事件,则阻止touch事件,优先执行scrollTo方法
                    e.preventDefault();
                }
                // 记录startTop, 并且只有startTop存在值时才允许move
                _this2.startTop = scrollWrap.scrollTop;

                // startY用来计算距离
                _this2.startY = e.touches ? e.touches[0].pageY : e.clientY;
                // X的作用是用来计算方向，如果是横向，则不进行动画处理，避免误操作
                _this2.startX = e.touches ? e.touches[0].pageX : e.clientX;
            };

            scrollWrap.addEventListener('touchstart', touchstartEvent);
            scrollWrap.addEventListener('mousedown', touchstartEvent);

            // 触摸结束
            var touchendEvent = function touchendEvent() {
                var options = _this2.options;

                // 需要重置状态
                if (_this2.isMoveDown) {
                    // 如果下拉区域已经执行动画,则需重置回来
                    if (_this2.downHight >= options.down.offset) {
                        // 符合触发刷新的条件
                        _this2.triggerDownLoading();
                    } else {
                        // 否则默认重置位置
                        _this2.translateContentWrap(0, options.down.bounceTime);
                        _this2.events[EVENT_CANCEL_LOADING] && _this2.events[EVENT_CANCEL_LOADING]();
                    }

                    _this2.isMoveDown = false;
                }

                _this2.startY = 0;
                _this2.startX = 0;
                _this2.preY = 0;
                _this2.startTop = undefined;
                // 当前是否正处于回弹中，常用于iOS中判断，如果先上拉再下拉就处于回弹中（只要moveY为负）
                _this2.isBounce = false;
            };

            scrollWrap.addEventListener('touchend', touchendEvent);
            scrollWrap.addEventListener('mouseup', touchendEvent);
            scrollWrap.addEventListener('mouseleave', touchendEvent);

            // 触摸中
            var touchmoveEvent = function touchmoveEvent(e) {
                var options = _this2.options;
                var isAllowDownloading = true;

                if (_this2.downLoading) {
                    isAllowDownloading = false;
                } else if (!options.down.isAways && _this2.upLoading) {
                    isAllowDownloading = false;
                }

                if (_this2.startTop !== undefined && _this2.startTop <= 0 && isAllowDownloading && !_this2.options.down.isLock) {
                    // 列表在顶部且不在加载中，并且没有锁住下拉动画

                    // 当前第一个手指距离列表顶部的距离
                    var curY = e.touches ? e.touches[0].pageY : e.clientY;
                    var curX = e.touches ? e.touches[0].pageX : e.clientX;

                    // 手指滑出屏幕触发刷新
                    if (curY > docClientHeight) {
                        touchendEvent(e);

                        return;
                    }

                    if (!_this2.preY) {
                        // 设置上次移动的距离，作用是用来计算滑动方向
                        _this2.preY = curY;
                    }

                    // 和上次比,移动的距离 (大于0向下,小于0向上)
                    var diff = curY - _this2.preY;

                    _this2.preY = curY;

                    // 和起点比,移动的距离,大于0向下拉
                    var moveY = curY - _this2.startY;
                    var moveX = curX - _this2.startX;

                    // 如果锁定横向滑动并且横向滑动更多，阻止默认事件
                    if (options.isLockX && Math.abs(moveX) > Math.abs(moveY)) {
                        e.preventDefault();

                        return;
                    }

                    if (_this2.isBounce && _this2.os.ios) {
                        // 暂时iOS中去回弹
                        // 下一个版本中，分开成两种情况，一种是absolute的固定动画，一种是在scrollWrap内部跟随滚动的动画
                        return;
                    }

                    if (moveY > 0) {
                        // 向下拉
                        _this2.isMoveDown = true;

                        // 阻止浏览器的默认滚动事件，因为这时候只需要执行动画即可
                        e.preventDefault();

                        if (!_this2.downHight) {
                            // 下拉区域的高度，用translate动画
                            _this2.downHight = 0;
                        }

                        var downOffset = options.down.offset;
                        var dampRate = 1;

                        if (_this2.downHight < downOffset) {
                            // 下拉距离  < 指定距离
                            dampRate = options.down.dampRateBegin;
                        } else {
                            // 超出了指定距离，随时可以刷新
                            dampRate = options.down.dampRate;
                        }

                        if (diff > 0) {
                            // 需要加上阻尼系数
                            _this2.downHight += diff * dampRate;
                        } else {
                            // 向上收回高度,则向上滑多少收多少高度
                            _this2.downHight += diff;
                        }

                        _this2.events[EVENT_PULL] && _this2.events[EVENT_PULL](_this2.downHight, downOffset);

                        // 执行动画
                        _this2.translateContentWrap(_this2.downHight);
                    } else {
                        _this2.isBounce = true;
                        // 解决嵌套问题。在嵌套有 IScroll，或类似的组件时，这段代码会生效，可以辅助滚动scrolltop
                        // 否则有可能在最开始滚不动
                        if (scrollWrap.scrollTop <= 0) {
                            scrollWrap.scrollTop += Math.abs(diff);
                        }
                    }
                }
            };

            scrollWrap.addEventListener('touchmove', touchmoveEvent);
            scrollWrap.addEventListener('mousemove', touchmoveEvent);
        }
    }, {
        key: '_initPullUp',
        value: function _initPullUp() {
            var _this3 = this;

            var scrollWrap = this.scrollWrap;

            // 如果是Body上的滑动，需要监听window的scroll
            var targetScrollDom = scrollWrap === document.body ? window : scrollWrap;

            targetScrollDom.addEventListener('scroll', function () {
                var scrollTop = scrollWrap.scrollTop;
                var scrollHeight = scrollWrap.scrollHeight;
                var clientHeight = getClientHeightByDom(scrollWrap);
                var options = _this3.options;

                _this3.events[EVENT_SCROLL] && _this3.events[EVENT_SCROLL](scrollTop);

                var isAllowUploading = true;

                if (_this3.upLoading) {
                    isAllowUploading = false;
                } else if (!options.down.isAways && _this3.downLoading) {
                    isAllowUploading = false;
                }

                if (isAllowUploading) {
                    if (!_this3.options.up.isLock && !_this3.isFinishUp && scrollHeight > 0) {
                        var toBottom = scrollHeight - clientHeight - scrollTop;

                        if (toBottom <= options.up.offset) {
                            // 满足上拉加载
                            _this3.triggerUpLoading();
                        }
                    }
                }
            });
        }
    }, {
        key: '_loadFull',
        value: function _loadFull() {
            var _this4 = this;

            var scrollWrap = this.scrollWrap;
            var options = this.options;

            setTimeout(function () {
                // 在下一个循环中运行
                if (!_this4.options.up.isLock && options.up.loadFull.isEnable
                // 避免无法计算高度时无限加载
                && scrollWrap.scrollTop === 0
                // scrollHeight是网页内容高度（最小值是clientHeight）
                && scrollWrap.scrollHeight > 0 && scrollWrap.scrollHeight <= getClientHeightByDom(scrollWrap)) {
                    _this4.triggerUpLoading();
                }
            }, options.up.loadFull.delay || 0);
        }
    }, {
        key: 'triggerDownLoading',
        value: function triggerDownLoading() {
            var options = this.options;

            if (!this.hooks[HOOK_BEFORE_DOWN_LOADING] || this.hooks[HOOK_BEFORE_DOWN_LOADING](this.downHight, options.down.offset)) {
                // 没有hook或者hook返回true都通过，主要是为了方便类似于秘密花园等的自定义下拉刷新动画实现
                this.downLoading = true;
                this.translateContentWrap(options.down.offset, options.down.bounceTime);

                this.events[EVENT_DOWN_LOADING] && this.events[EVENT_DOWN_LOADING]();
            }
        }
    }, {
        key: 'endDownLoading',
        value: function endDownLoading() {
            var options = this.options;

            if (this.downLoading) {
                // 必须是loading时才允许结束
                this.translateContentWrap(0, options.down.bounceTime);
                this.downLoading = false;
            }
        }
    }, {
        key: 'triggerUpLoading',
        value: function triggerUpLoading() {
            this.upLoading = true;
            this.events[EVENT_UP_LOADING] && this.events[EVENT_UP_LOADING]();
        }

        /**
         * 结束上拉加载动画时需要判断是否已经finished(不能加载更多，没有数据了)
         * @param {Boolean} isFinishUp 是否结束上拉加载
         */

    }, {
        key: 'endUpLoading',
        value: function endUpLoading(isFinishUp) {
            if (this.upLoading) {
                this.upLoading = false;

                if (isFinishUp) {
                    this.isFinishUp = true;
                } else {
                    this._loadFull();
                }
            }
        }
    }, {
        key: 'resetUpLoading',
        value: function resetUpLoading() {
            if (this.isFinishUp) {
                this.isFinishUp = false;
            }

            // 检测是否需要加载满屏
            this._loadFull();

            this.events[EVENT_RESET_UP_LOADING] && this.events[EVENT_RESET_UP_LOADING]();
        }

        /**
         * 滚动到指定的y位置
         * @param {Number} y top坐标
         * @param {Number} duration 单位毫秒
         */

    }, {
        key: 'scrollTo',
        value: function scrollTo(y, duration) {
            var _this5 = this;

            var scrollWrap = this.scrollWrap;
            var translateDuration = duration || 0;
            // 最大可滚动的y
            var maxY = scrollWrap.scrollHeight - getClientHeightByDom(scrollWrap);
            var translateY = y || 0;

            translateY = Math.max(translateY, 0);
            translateY = Math.min(translateY, maxY);

            // 差值 (可能为负)
            var diff = scrollWrap.scrollTop - translateY;

            if (diff === 0 || translateDuration === 0) {
                scrollWrap.scrollTop = translateY;

                return;
            }

            // 每秒60帧，计算一共多少帧，然后每帧的步长
            var count = Math.floor(translateDuration / PER_SECOND);
            var step = diff / count;
            var curr = 0;

            var execute = function execute() {
                if (curr < count) {
                    if (curr === count - 1) {
                        // 最后一次直接设置y,避免计算误差
                        scrollWrap.scrollTop = translateY;
                    } else {
                        scrollWrap.scrollTop -= step;
                    }
                    curr += 1;
                    requestAnimationFrame(execute);
                } else {
                    scrollWrap.scrollTop = translateY;
                    _this5.isScrollTo = false;
                }
            };

            // 锁定状态
            this.isScrollTo = true;
            requestAnimationFrame(execute);
        }

        /**
         * 监听事件，包括下拉过程，下拉刷新，上拉加载，滑动等事件都可以监听到
         * @param {String} event 事件名，可选名称
         * 在最上方的常量有定义
         * @param {Function} callback 回调函数
         */

    }, {
        key: 'on',
        value: function on(event, callback) {
            if (event && typeof callback === 'function') {
                this.events[event] = callback;
            }
        }

        /**
         * 注册钩子函数，主要是一些自定义刷新动画时用到，如进入秘密花园
         * @param {String} hook 名称，范围如下
         * beforeDownLoading 是否准备downLoading，如果返回false，则不会loading，完全进入自定义动画
         * @param {Function} callback 回调函数
         */

    }, {
        key: 'hook',
        value: function hook(_hook, callback) {
            if (_hook && typeof callback === 'function') {
                this.hooks[_hook] = callback;
            }
        }
    }]);

    return Scroll;
}();

var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

var defaultSetting = {
    // 下拉有关
    down: {
        // 默认没有锁定，可以通过API动态设置
        isLock: false,
        // 是否自动下拉刷新
        isAuto: false,
        // 设置isAuto=true时生效，是否在初始化的下拉刷新触发事件中显示动画，如果是false，初始化的加载只会触发回调，不会触发动画
        isAllowAutoLoading: true,
        // 是否不管任何情况下都能触发下拉刷新，为false的话当上拉时不会触发下拉
        isAways: false,
        // 是否scroll在下拉时会进行移动(css3)，通过关闭它可以实现自定义动画
        isScrollCssTranslate: true,
        // 是否每次下拉完毕后默认重置上拉
        isAutoResetUpLoading: true,
        // 下拉要大于多少长度后再下拉刷新
        offset: 75,
        // 阻尼系数，下拉小于offset时的阻尼系数，值越接近0,高度变化越小,表现为越往下越难拉
        dampRateBegin: 1,
        // 阻尼系数，下拉的距离大于offset时,改变下拉区域高度比例;值越接近0,高度变化越小,表现为越往下越难拉
        dampRate: 0.3,
        // 回弹动画时间
        bounceTime: 300,
        successAnim: {
            // 下拉刷新结束后是否有成功动画，默认为false，如果想要有成功刷新xxx条数据这种操作，请设为true，并实现对应hook函数
            isEnable: false,
            duration: 300
        },
        // 下拉时会提供回调，默认为null不会执行
        onPull: null,
        // 取消时回调
        onCalcel: null,
        callback: noop
    },
    // 上拉有关
    up: {
        // 默认没有锁定，可以通过API动态设置
        isLock: false,
        // 是否自动上拉加载-初始化是是否自动
        isAuto: true,
        // 是否默认显示上拉进度条，可以通过API改变
        isShowUpLoading: true,
        // 距离底部高度(到达该高度即触发)
        offset: 100,
        loadFull: {
            // 开启配置后，只要没满屏幕，就会自动加载
            isEnable: true,
            delay: 300
        },
        // 滚动时会提供回调，默认为null不会执行
        onScroll: null,
        callback: noop
    },
    // 容器
    container: '#minirefresh',
    // 是否锁定横向滑动，如果锁定则原生滚动条无法滑动
    isLockX: true,
    // 是否显示滚动条
    isScrollBar: true,
    // 是否使用body对象的scroll而不是minirefresh-scroll对象的scroll
    // 开启后一个页面只能有一个下拉刷新，否则会有冲突
    isUseBodyScroll: false
};

var CLASS_HIDDEN_SCROLLBAR = 'minirefresh-hide-scrollbar';

var Core = function () {
    /**
     * 构造函数
     * @param {Object} options 配置信息
     * @constructor
     */
    function Core(options) {
        _classCallCheck(this, Core);

        osMixin(this);
        this.options = extend(true, {}, defaultSetting, options);

        this.container = selector(this.options.container);
        // scroll的dom-wrapper下的第一个节点，作用是down动画时的操作
        this.contentWrap = this.container.children[0];
        // 默认是整个container进行滚动
        // 但是为了兼容body的滚动，拆分为两个对象方便处理
        // 如果是使用body的情况，scrollWrap恒为body
        // 注意，滑动不是指下拉时的translate（这时候时contentWrap），而是只默认的原生滑动
        this.scrollWrap = this.options.isUseBodyScroll ? document.body : this.container;

        if (!this.options.isScrollBar) {
            this.container.classList.add(CLASS_HIDDEN_SCROLLBAR);
        }

        // 初始化的hook
        this._initHook && this._initHook(this.options.down.isLock, this.options.up.isLock);

        // 生成一个Scroll对象 ，对象内部处理滑动监听
        this.scroller = new Scroll(this);

        // 内部处理scroll
        this._initEvent();
        // 如果初始化时锁定了，需要触发锁定，避免没有锁定时解锁（会触发逻辑bug）
        this.options.up.isLock && this._lockUpLoading(this.options.up.isLock);
        this.options.down.isLock && this._lockDownLoading(this.options.down.isLock);
    }

    _createClass(Core, [{
        key: '_initEvent',
        value: function _initEvent() {
            var _this = this;

            // 缓存options，这部分的配置是不允许reset的
            var options = this.options;

            this.scroller.on('initScroll', function () {
                _this._initScrollHook && _this._initScrollHook();
            });
            this.scroller.on('downLoading', function (isHideLoading) {
                !isHideLoading && _this._downLoaingHook && _this._downLoaingHook();
                options.down.callback && options.down.callback();
            });
            this.scroller.on('cancelLoading', function () {
                _this._cancelLoaingHook && _this._cancelLoaingHook();
                options.down.onCalcel && options.down.onCalcel();
            });
            this.scroller.on('pull', function (downHight, downOffset) {
                _this._pullHook && _this._pullHook(downHight, downOffset);
                options.down.onPull && options.down.onPull(downHight, downOffset);
            });
            this.scroller.on('upLoading', function () {
                _this._upLoaingHook && _this._upLoaingHook(_this.options.up.isShowUpLoading);
                options.up.callback && options.up.callback(_this.options.up.isShowUpLoading);
            });
            this.scroller.on('resetUpLoading', function () {
                _this._resetUpLoadingHook && _this._resetUpLoadingHook();
            });
            this.scroller.on('scroll', function (scrollTop) {
                _this._scrollHook && _this._scrollHook(scrollTop);
                options.up.onScroll && options.up.onScroll(scrollTop);
            });

            // 检查是否允许普通的加载中，如果返回false，就代表自定义下拉刷新，通常自己处理
            this.scroller.hook('beforeDownLoading', function (downHight, downOffset) {
                return !_this._beforeDownLoadingHook || _this._beforeDownLoadingHook(downHight, downOffset);
            });
        }

        /**
         * 内部执行，结束下拉刷新
         * @param {Boolean} isSuccess 是否下拉请求成功
         * @param {String} successTips 需要更新的成功提示
         * 在开启了成功动画时，往往成功的提示是需要由外传入动态更新的，譬如  update 10 news
         */

    }, {
        key: '_endDownLoading',
        value: function _endDownLoading(isSuccess, successTips) {
            var _this2 = this;

            if (!this.options.down) {
                // 防止没传down导致错误
                return;
            }

            if (this.scroller.downLoading) {
                // 必须是loading时才允许执行对应hook
                var successAnim = this.options.down.successAnim.isEnable;
                var successAnimTime = this.options.down.successAnim.duration;

                if (successAnim) {
                    // 如果有成功动画
                    this._downLoaingSuccessHook && this._downLoaingSuccessHook(isSuccess, successTips);
                } else {
                    // 默认为没有成功动画
                    successAnimTime = 0;
                }

                setTimeout(function () {
                    // 成功动画结束后就可以重置位置了
                    _this2.scroller.endDownLoading();
                    // 触发结束hook
                    _this2._downLoaingEndHook && _this2._downLoaingEndHook(isSuccess);
                }, successAnimTime);
            }
        }

        /**
         * 锁定上拉加载
         * 将开启和禁止合并成一个锁定API
         * @param {Boolean} isLock 是否锁定
         */

    }, {
        key: '_lockUpLoading',
        value: function _lockUpLoading(isLock) {
            this.options.up.isLock = isLock;
            this._lockUpLoadingHook && this._lockUpLoadingHook(isLock);
        }

        /**
         * 锁定下拉刷新
         * @param {Boolean} isLock 是否锁定
         */

    }, {
        key: '_lockDownLoading',
        value: function _lockDownLoading(isLock) {
            this.options.down.isLock = isLock;
            this._lockDownLoadingHook && this._lockDownLoadingHook(isLock);
        }

        /**
         * 刷新minirefresh的配置，关键性的配置请不要更新，如容器，回调等
         * @param {Object} options 新的配置，会覆盖原有的
         */

    }, {
        key: 'refreshOptions',
        value: function refreshOptions(options) {
            this.options = extend(true, {}, this.options, options);
            this.scroller.refreshOptions(this.options);
            this._lockUpLoading(this.options.up.isLock);
            this._lockDownLoading(this.options.down.isLock);
            this._refreshHook && this._refreshHook();
        }

        /**
         * 结束下拉刷新
         * @param {Boolean} isSuccess 是否请求成功，这个状态会中转给对应主题
         * @param {String} successTips 需要更新的成功提示
         * 在开启了成功动画时，往往成功的提示是需要由外传入动态更新的，譬如  update 10 news
         */

    }, {
        key: 'endDownLoading',
        value: function endDownLoading() {
            var isSuccess = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : true;
            var successTips = arguments[1];

            this._endDownLoading(isSuccess, successTips);
            // 同时恢复上拉加载的状态，注意，此时没有传isShowUpLoading，所以这个值不会生效
            if (this.options.down.isAutoResetUpLoading) {
                this.resetUpLoading();
            }
        }

        /**
         * 重置上拉加载状态,如果是没有更多数据后重置，会变为可以继续上拉加载
         */

    }, {
        key: 'resetUpLoading',
        value: function resetUpLoading() {
            this.scroller.resetUpLoading();
        }

        /**
         * 结束上拉加载
         * @param {Boolean} isFinishUp 是否结束上拉加载，如果结束，就相当于变为了没有更多数据，无法再出发上拉加载了
         * 结束后必须reset才能重新开启
         */

    }, {
        key: 'endUpLoading',
        value: function endUpLoading(isFinishUp) {
            if (this.scroller.upLoading) {
                this.scroller.endUpLoading(isFinishUp);
                this._upLoaingEndHook && this._upLoaingEndHook(isFinishUp);
            }
        }
    }, {
        key: 'triggerUpLoading',
        value: function triggerUpLoading() {
            this.scroller.triggerUpLoading();
        }
    }, {
        key: 'triggerDownLoading',
        value: function triggerDownLoading() {
            this.scroller.scrollTo(0);
            this.scroller.triggerDownLoading();
        }

        /**
         * 滚动到指定的y位置
         * @param {Number} y 需要滑动到的top值
         * @param {Number} duration 单位毫秒
         */

    }, {
        key: 'scrollTo',
        value: function scrollTo(y, duration) {
            this.scroller.scrollTo(y, duration);
        }

        /**
         * 获取当前的滚动位置
         * @return {Number} 返回当前的滚动位置
         */

    }, {
        key: 'getPosition',
        value: function getPosition() {
            return this.scrollWrap.scrollTop;
        }
    }]);

    return Core;
}();

var MiniRefreshTools$2 = {};

Object.keys(lang).forEach(function (name) {
    MiniRefreshTools$2[name] = lang[name];
});

// namespace的特殊把绑定
MiniRefreshTools$2.namespace = function (namespaceStr, target) {
    namespace(MiniRefreshTools$2, namespaceStr, target);
};

MiniRefreshTools$2.Core = Core;
MiniRefreshTools$2.version = '2.0.0';

// 防止主题和核心一起，并且require模式中，无法全局变量的情况
window.MiniRefreshTools = MiniRefreshTools$2;

var _createClass$2 = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _classCallCheck$2(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var Core$2 = MiniRefreshTools.Core;
var version = MiniRefreshTools.version;
var extend$1 = MiniRefreshTools.extend;
var namespace$1 = MiniRefreshTools.namespace;

/**
 * 一些默认提供的CSS类，一般来说不会变动（由框架提供的）
 * THEME 字段会根据不同的主题有不同值
 * 在使用body的scroll时，需要加上样式 CLASS_BODY_SCROLL_WRAP
 */
var CLASS_THEME = 'minirefresh-theme-default';
var CLASS_DOWN_WRAP = 'minirefresh-downwrap';
var CLASS_UP_WRAP = 'minirefresh-upwrap';
var CLASS_FADE_IN = 'minirefresh-fade-in';
var CLASS_FADE_OUT = 'minirefresh-fade-out';
var CLASS_TO_TOP = 'minirefresh-totop';
var CLASS_ROTATE = 'minirefresh-rotate';
var CLASS_HARDWARE_SPEEDUP = 'minirefresh-hardware-speedup';
var CLASS_HIDDEN = 'minirefresh-hidden';
var CLASS_BODY_SCROLL_WRAP = 'body-scroll-wrap';

/**
 * 本主题的特色样式
 */
var CLASS_DOWN_SUCCESS = 'downwrap-success';
var CLASS_DOWN_ERROR = 'downwrap-error';
var CLASS_STATUS_DEFAULT = 'status-default';
var CLASS_STATUS_PULL = 'status-pull';
var CLASS_STATUS_LOADING = 'status-loading';
var CLASS_STATUS_SUCCESS = 'status-success';
var CLASS_STATUS_ERROR = 'status-error';
var CLASS_STATUS_NOMORE = 'status-nomore';

/**
 * 一些常量
 */
var DEFAULT_DOWN_HEIGHT = 75;

var defaultSetting$1 = {
    down: {
        successAnim: {
            // 下拉刷新结束后是否有成功动画，默认为false，如果想要有成功刷新xxx条数据这种操作，请设为true，并实现对应hook函数
            isEnable: false,
            duration: 300
        },
        // 可选，在下拉可刷新状态时，下拉刷新控件上显示的标题内容
        contentdown: '下拉刷新',
        // 可选，在释放可刷新状态时，下拉刷新控件上显示的标题内容
        contentover: '释放刷新',
        // 可选，正在刷新状态时，下拉刷新控件上显示的标题内容
        contentrefresh: '正在加载...',
        // 可选，刷新成功的提示，当开启successAnim时才有效
        contentsuccess: '刷新成功',
        // 可选，刷新失败的提示，错误回调用到，当开启successAnim时才有效
        contenterror: '刷新失败',
        // 是否默认跟随进行css动画
        isWrapCssTranslate: false
    },
    up: {
        toTop: {
            // 是否开启点击回到顶部
            isEnable: true,
            duration: 300,
            // 滚动多少距离才显示toTop
            offset: 800
        },
        // 默认为空，可以自行改为 上拉显示更多 等
        contentdown: '正在加载...',
        contentrefresh: '正在加载...',
        contentnomore: ''//没有更多数据了
    }
};

var MiniRefreshTheme = function (_Core) {
    _inherits(MiniRefreshTheme, _Core);

    /**
     * 构造，使用新的默认参数
     * @param {Object} options 配置信息
     * @constructor
     */
    function MiniRefreshTheme(options) {
        _classCallCheck$2(this, MiniRefreshTheme);

        var newOptions = extend$1(true, {}, defaultSetting$1, options);

        return _possibleConstructorReturn(this, (MiniRefreshTheme.__proto__ || Object.getPrototypeOf(MiniRefreshTheme)).call(this, newOptions));
    }

    _createClass$2(MiniRefreshTheme, [{
        key: '_initHook',
        value: function _initHook() {
            var container = this.container;
            var contentWrap = this.contentWrap;

            container.classList.add(CLASS_THEME);
            // 加上硬件加速让动画更流畅
            contentWrap.classList.add(CLASS_HARDWARE_SPEEDUP);

            if (this.options.isUseBodyScroll) {
                // 如果使用了body的scroll，需要增加对应的样式，否则默认的absolute无法被监听到
                container.classList.add(CLASS_BODY_SCROLL_WRAP);
                contentWrap.classList.add(CLASS_BODY_SCROLL_WRAP);
            }

            this._initDownWrap();
            this._initUpWrap();
            this._initToTop();
        }

        /**
         * 刷新的实现，需要根据新配置进行一些更改
         */

    }, {
        key: '_refreshHook',
        value: function _refreshHook() {
            // 如果开关csstranslate，需要兼容
            if (this.options.down.isWrapCssTranslate) {
                this._transformDownWrap(-this.downWrapHeight);
            } else {
                this._transformDownWrap(0, 0, true);
            }

            // toTop的显影控制，如果本身显示了，又更新为隐藏，需要马上隐藏
            if (!this.options.up.toTop.isEnable) {
                this.toTopBtn && this.toTopBtn.classList.add(CLASS_HIDDEN);
                this.isShowToTopBtn = false;
            }
        }
    }, {
        key: '_initDownWrap',
        value: function _initDownWrap() {
            var container = this.container;
            var contentWrap = this.contentWrap;
            var options = this.options;

            // 下拉的区域
            var downWrap = document.createElement('div');

            downWrap.className = CLASS_DOWN_WRAP + ' ' + CLASS_HARDWARE_SPEEDUP;
            downWrap.innerHTML = ' \n            <div class="downwrap-content">\n                <p class="downwrap-progress"></p>\n                <p class="downwrap-tips">' + options.down.contentdown + '</p>\n            </div>\n        ';
            container.insertBefore(downWrap, contentWrap);

            this.downWrap = downWrap;
            this.downWrapProgress = this.downWrap.querySelector('.downwrap-progress');
            this.downWrapTips = this.downWrap.querySelector('.downwrap-tips');
            // 是否能下拉的变量，控制pull时的状态转变
            this.isCanPullDown = false;
            this.downWrapHeight = downWrap.offsetHeight || DEFAULT_DOWN_HEIGHT;
            this._transformDownWrap(-this.downWrapHeight);
            MiniRefreshTheme._changeWrapStatusClass(this.downWrap, CLASS_STATUS_DEFAULT);
        }
    }, {
        key: '_transformDownWrap',
        value: function _transformDownWrap() {
            var offset = arguments.length > 0 && arguments[0] !== undefined ? arguments[0] : 0;
            var duration = arguments.length > 1 && arguments[1] !== undefined ? arguments[1] : 0;
            var isForce = arguments[2];

            if (!isForce && !this.options.down.isWrapCssTranslate) {
                // 哪怕关闭了isWrapCssTranslate，也可以通过isForce参数强制移动
                return;
            }

            var duratuinStr = duration + 'ms';
            var transformStr = 'translateY(' + offset + 'px)  translateZ(0px)';

            // 记得动画时 translateZ 否则硬件加速会被覆盖
            this.downWrap.style.webkitTransitionDuration = duratuinStr;
            this.downWrap.style.transitionDuration = duratuinStr;
            this.downWrap.style.webkitTransform = transformStr;
            this.downWrap.style.transform = transformStr;
        }
    }, {
        key: '_initUpWrap',
        value: function _initUpWrap() {
            var contentWrap = this.contentWrap;
            var options = this.options;

            // 上拉区域
            var upWrap = document.createElement('div');

            upWrap.className = CLASS_UP_WRAP + ' ' + CLASS_HARDWARE_SPEEDUP;
            upWrap.innerHTML = ' \n            <p class="upwrap-progress"></p>\n            <p class="upwrap-tips">' + options.up.contentdown + '</p>\n        ';

            upWrap.style.visibility = 'hidden';
            // 加到container中
            contentWrap.appendChild(upWrap);

            this.upWrap = upWrap;
            this.upWrapProgress = this.upWrap.querySelector('.upwrap-progress');
            this.upWrapTips = this.upWrap.querySelector('.upwrap-tips');
            MiniRefreshTheme._changeWrapStatusClass(this.upWrap, CLASS_STATUS_DEFAULT);
        }

        /**
         * 自定义实现一个toTop，由于这个是属于额外的事件所以没有添加的核心中，而是由各自的主题决定是否实现或者实现成什么样子
         * 不过框架中仍然提供了一个默认的minirefresh-totop样式，可以方便使用
         */

    }, {
        key: '_initToTop',
        value: function _initToTop() {
            var _this2 = this;

            var options = this.options;
            var toTop = options.up.toTop.isEnable;
            var duration = options.up.toTop.duration;

            if (toTop) {
                var toTopBtn = document.createElement('div');

                toTopBtn.className = CLASS_TO_TOP + ' ' + CLASS_THEME;

                toTopBtn.onclick = function () {
                    _this2.scroller.scrollTo(0, duration);
                };
                toTopBtn.classList.add(CLASS_HIDDEN);
                this.toTopBtn = toTopBtn;
                this.isShowToTopBtn = false;
                // 默认添加到body中防止冲突
                // 需要添加到container，否则多个totop无法识别
                this.container.appendChild(toTopBtn);
            }
        }
    }, {
        key: '_pullHook',
        value: function _pullHook(downHight, downOffset) {
            var options = this.options;
            var FULL_DEGREE = 360;

            if (downHight < downOffset) {
                if (this.isCanPullDown) {
                    this.isCanPullDown = false;
                    MiniRefreshTheme._changeWrapStatusClass(this.downWrap, CLASS_STATUS_DEFAULT);
                    this.downWrapTips.innerText = options.down.contentdown;
                }
            } else if (!this.isCanPullDown) {
                this.downWrapTips.innerText = options.down.contentover;
                this.isCanPullDown = true;
                MiniRefreshTheme._changeWrapStatusClass(this.downWrap, CLASS_STATUS_PULL);
            }

            if (this.downWrapProgress) {
                var rate = downHight / downOffset;
                var progress = FULL_DEGREE * rate;
                var rotateStr = 'rotate(' + progress + 'deg)';

                this.downWrapProgress.style.webkitTransform = rotateStr;
                this.downWrapProgress.style.transform = rotateStr;
            }

            this._transformDownWrap(-this.downWrapHeight + downHight);
        }
    }, {
        key: '_scrollHook',
        value: function _scrollHook(scrollTop) {
            // 用来判断toTop
            var options = this.options;
            var toTop = options.up.toTop.isEnable;
            var toTopBtn = this.toTopBtn;

            if (toTop && toTopBtn) {
                if (scrollTop >= options.up.toTop.offset) {
                    if (!this.isShowToTopBtn) {
                        toTopBtn.classList.remove(CLASS_FADE_OUT);
                        toTopBtn.classList.remove(CLASS_HIDDEN);
                        toTopBtn.classList.add(CLASS_FADE_IN);
                        this.isShowToTopBtn = true;
                    }
                } else if (this.isShowToTopBtn) {
                    toTopBtn.classList.add(CLASS_FADE_OUT);
                    toTopBtn.classList.remove(CLASS_FADE_IN);
                    this.isShowToTopBtn = false;
                }
            }
        }
    }, {
        key: '_downLoaingHook',
        value: function _downLoaingHook() {
            // 默认和contentWrap的同步
            this._transformDownWrap(-this.downWrapHeight + this.options.down.offset, this.options.down.bounceTime);
            this.downWrapTips.innerText = this.options.down.contentrefresh;
            this.downWrapProgress.classList.add(CLASS_ROTATE);
            MiniRefreshTheme._changeWrapStatusClass(this.downWrap, CLASS_STATUS_LOADING);
        }
    }, {
        key: '_downLoaingSuccessHook',
        value: function _downLoaingSuccessHook(isSuccess, successTips) {
            this.options.down.contentsuccess = successTips || this.options.down.contentsuccess;
            this.downWrapTips.innerText = isSuccess ? this.options.down.contentsuccess : this.options.down.contenterror;
            this.downWrapProgress.classList.remove(CLASS_ROTATE);
            this.downWrapProgress.classList.add(CLASS_FADE_OUT);
            this.downWrapProgress.classList.add(isSuccess ? CLASS_DOWN_SUCCESS : CLASS_DOWN_ERROR);

            MiniRefreshTheme._changeWrapStatusClass(this.downWrap, isSuccess ? CLASS_STATUS_SUCCESS : CLASS_STATUS_ERROR);
        }
    }, {
        key: '_downLoaingEndHook',
        value: function _downLoaingEndHook(isSuccess) {
            this.downWrapTips.innerText = this.options.down.contentdown;
            this.downWrapProgress.classList.remove(CLASS_ROTATE);
            this.downWrapProgress.classList.remove(CLASS_FADE_OUT);
            this.downWrapProgress.classList.remove(isSuccess ? CLASS_DOWN_SUCCESS : CLASS_DOWN_ERROR);
            // 默认为不可见
            // 需要重置回来
            this.isCanPullDown = false;
            this._transformDownWrap(-this.downWrapHeight, this.options.down.bounceTime);
            MiniRefreshTheme._changeWrapStatusClass(this.downWrap, CLASS_STATUS_DEFAULT);
        }
    }, {
        key: '_cancelLoaingHook',
        value: function _cancelLoaingHook() {
            this._transformDownWrap(-this.downWrapHeight, this.options.down.bounceTime);
            MiniRefreshTheme._changeWrapStatusClass(this.downWrap, CLASS_STATUS_DEFAULT);
        }
    }, {
        key: '_upLoaingHook',
        value: function _upLoaingHook(isShowUpLoading) {
            if (isShowUpLoading) {
                this.upWrapTips.innerText = this.options.up.contentrefresh;
                this.upWrapProgress.classList.add(CLASS_ROTATE);
                this.upWrapProgress.classList.remove(CLASS_HIDDEN);
                this.upWrap.style.visibility = 'visible';
            } else {
                this.upWrap.style.visibility = 'hidden';
            }
            MiniRefreshTheme._changeWrapStatusClass(this.upWrap, CLASS_STATUS_LOADING);
        }
    }, {
        key: '_upLoaingEndHook',
        value: function _upLoaingEndHook(isFinishUp) {
            if (!isFinishUp) {
                // 接下来还可以加载更多
                // this.upWrap.style.visibility = 'hidden';
                this.upWrapTips.innerText = this.options.up.contentdown;
                MiniRefreshTheme._changeWrapStatusClass(this.upWrap, CLASS_STATUS_DEFAULT);
            } else {
                // 已经没有更多数据了
                // this.upWrap.style.visibility = 'visible';
                this.upWrapTips.innerText = this.options.up.contentnomore;
                MiniRefreshTheme._changeWrapStatusClass(this.upWrap, CLASS_STATUS_NOMORE);
            }
            this.upWrapProgress.classList.remove(CLASS_ROTATE);
            this.upWrapProgress.classList.add(CLASS_HIDDEN);
        }
    }, {
        key: '_resetUpLoadingHook',
        value: function _resetUpLoadingHook() {
            // this.upWrap.style.visibility = 'hidden';
            this.upWrapTips.innerText = this.options.up.contentdown;
            this.upWrapProgress.classList.remove(CLASS_ROTATE);
            this.upWrapProgress.classList.add(CLASS_HIDDEN);
            MiniRefreshTheme._changeWrapStatusClass(this.upWrap, CLASS_STATUS_DEFAULT);
        }
    }, {
        key: '_lockUpLoadingHook',
        value: function _lockUpLoadingHook(isLock) {
            this.upWrap.style.visibility = isLock ? 'hidden' : 'visible';
        }
    }, {
        key: '_lockDownLoadingHook',
        value: function _lockDownLoadingHook(isLock) {
            this.downWrap.style.visibility = isLock ? 'hidden' : 'visible';
        }
    }], [{
        key: '_changeWrapStatusClass',
        value: function _changeWrapStatusClass(wrap, statusClass) {
            wrap.classList.remove(CLASS_STATUS_NOMORE);
            wrap.classList.remove(CLASS_STATUS_DEFAULT);
            wrap.classList.remove(CLASS_STATUS_PULL);
            wrap.classList.remove(CLASS_STATUS_LOADING);
            wrap.classList.remove(CLASS_STATUS_SUCCESS);
            wrap.classList.remove(CLASS_STATUS_ERROR);
            wrap.classList.add(statusClass);
        }
    }]);

    return MiniRefreshTheme;
}(Core$2);

MiniRefreshTheme.sign = 'default';
MiniRefreshTheme.version = version;
namespace$1('theme.defaults', MiniRefreshTheme);

// 覆盖全局变量
window.MiniRefresh = MiniRefreshTheme;

/**
 * 默认暴露的是MiniRefreshTools变量，各大主题都是挂在到上面的
 */

return MiniRefreshTools$2;

})));
/******************************************************************************************
*★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★
*声明ajax请求数据信息
*★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★
*******************************************************************************************/
var ThisRefresh = null;
/******************************************************************************************
*应用刷新功能
*******************************************************************************************/
;(function($) {
"use strict";
	$.fn.Refresh = function(options)
	{
		/***************************************************************
		*申明当前的页码信息
		****************************************************************/
		try{var FormatPager = parseInt(options['page']) || 0;}
		catch(err){}
		/***************************************************************
		*加载网页参数信息
		****************************************************************/
		var GetRequest = function(downOrUp)
		{
			if (downOrUp == 'down') {
				FormatPager = 1;// 下拉刷新页码设置
			} else {
				FormatPager=FormatPager+1;//上拉加载递增页码
			};
			/*******************************************************************
			* 申明发送数据内容
			********************************************************************/
			var SendOptions = {};
			SendOptions['url'] = options['url'] || '';
			SendOptions['type'] = SendOptions['type'] || 'get';
			SendOptions['dataType'] = SendOptions['dataType'] || 'html';
			SendOptions['data'] = {
				'page': (FormatPager || 0),
				't': new Date().getTime()
			};
			SendOptions['success'] = function(strJson)
			{
				try{
					if (downOrUp == 'down') 
					{
						try{$(options['contianer']).html(strJson);}catch(err){}
						try{ThisRefresh.endDownLoading(true);}catch(err){}
					}
					else
					{
						try{$(options['contianer']).append(strJson);}catch(err){}
						if(strJson!="")
						{
							try{ThisRefresh.endUpLoading(false);}catch(err){}
						}else{
							try{ThisRefresh.endUpLoading(true);}catch(err){}
						}
					}
				}catch(err){}
			};
			/*******************************************************************
			* 容错数据处理
			********************************************************************/
			SendOptions['error'] = function()
			{
				
				try{ErrorMessager('数据请求过程中发生错误或返回格式不正确,请重试!');}
				catch(err){}	
			};
			
			/*******************************************************************
			* 输出错误数据处理过程
			********************************************************************/
			var ErrorMessager = function(strResp){
				try{
					if(window.WinAlert!=undefined && window.WinAlert!=null 
					&& typeof(window.WinAlert)=='function'){
						window.WinAlert(strResp,'error');	
					}else if(parent.WinAlert!=undefined && parent.WinAlert!=null 
					&& typeof(parent.WinAlert)=='function'){
						parent.WinAlert(strResp,'error');	
					}else if(top.WinAlert!=undefined && top.WinAlert!=null 
					&& typeof(top.WinAlert)=='function'){
						top.WinAlert(strResp,'error');	
					}else{try{alert(strResp);}catch(err){};}
				}catch(err){}		
			}
			/*******************************************************************
			* 发送数据请求
			********************************************************************/
			try{
				if(SendOptions!=undefined && SendOptions!=null 
				&& typeof(SendOptions)=='object' && SendOptions['url']!=undefined 
				&& SendOptions['url']!=null && SendOptions['url']!="" && jQuery!=undefined 
				&& jQuery!=null && jQuery.ajax!=undefined && typeof(jQuery.ajax)=='function')
				{
					try{jQuery.ajax(SendOptions);}
					catch(err){alert(err.message);}
				};
			}catch(err){}
		};
		/*******************************************************************
		* 加载网页数据信息
		********************************************************************/
		var options = options || {};
		options['isLock'] = options['isLock'] || false;
		options['isAuto'] = options['isAuto'] || true;
		options['container'] = options['container'] || this;
		var contianer = this;
		if(contianer!=undefined && contianer!=null && options!=undefined 
		&& options!=null && options['url']!=undefined 
		&& options['url']!=null && options['url']!='')
		{
			var ThisRefresh = new MiniRefresh({
				container:contianer.id,
				down: {
					isLock: (options['isLock']) || false,//是否禁用下拉刷新
					callback: function () {
						try{
							var obj = setTimeout(function(){
								try{GetRequest('down');}catch(err){}	
								try{clearTimeout(obj);}catch(err){}
							},1000);	
						}catch(err){}
					}
				},
				up: {
					isAuto: (options['isAuto']) || true,
					callback: function (){
						try{
							var obj = setTimeout(function(){
								try{GetRequest('up');}catch(err){}	
								try{clearTimeout(obj);}catch(err){}
							},1000);	
						}catch(err){}
					}
				}
			});	
		}
	};
	
	$.fn.JSONRefresh = function(options)
	{
		var isComlate = true;
		var isUpLoading = true;
		/***************************************************************
		*申明当前的页码信息
		****************************************************************/
		var Pagenumber = (parseInt(options['page']) || 0);
		var Pagesize = (parseInt(options['pagesize']) || 10);
		/***************************************************************
		*判断是否存在下一页
		****************************************************************/
		var isNext = function(resCount)
		{
			if(parseInt(resCount)){
				if(resCount<=0){return false;}
				else if(resCount<=Pagesize){return false;}
				else if(resCount<=((Pagenumber+1)*Pagesize)){return false;}
				else{return true;}
			}else{return false;}
		}
		/***************************************************************
		*加载网页参数信息
		****************************************************************/
		var GetRequest = function(downOrUp)
		{
			if(isComlate==true)
			{
				if (downOrUp == 'down') {Pagenumber = 1;}
				else {Pagenumber=Pagenumber+1;};
				try{isComlate=false;}catch(err){}
				/*******************************************************************
				* 申明发送数据内容
				********************************************************************/
				var SendOptions = {};
				SendOptions['url'] = options['url'] || '';
				SendOptions['type'] = SendOptions['type'] || 'get';
				SendOptions['dataType'] = SendOptions['dataType'] || 'json';
				SendOptions['data'] = {
					'page': (parseInt(Pagenumber) || 1),
					'pagesize': (parseInt(Pagesize) || 10),
					't': new Date().getTime()
				};
				SendOptions['back'] = function(rspJson)
				{
					try{isComlate=true;}
					catch(err){}
					if (downOrUp == 'down')
					{
						var record = parseInt(rspJson['record']) || 0;
						if(isNext(record)){
							try{isUpLoading=true;}catch(err){}
							try{ThisRefresh.endUpLoading(false);}catch(err){};
						}
						else{
							try{isUpLoading=false;}catch(err){}
							try{ThisRefresh.endUpLoading(true);}catch(err){};
						}
						try{ThisRefresh.endDownLoading(true);}catch(err){};
					}
					else
					{
						var record = parseInt(rspJson['record']) || 0;
						if(isNext(record)){
							try{isUpLoading=true;}catch(err){}
							try{ThisRefresh.endUpLoading(false);}catch(err){};
						}
						else{
							try{isUpLoading=false;}catch(err){}
							try{ThisRefresh.endUpLoading(true);}catch(err){};
						}
					};
					if(options['success']!=undefined && options['success']!=null
					&& typeof(options['success'])=='function')
					{
						var isAppend = (Pagenumber<=1) ? false : true;
						options['success'](rspJson,isAppend);
					};
				};
				/*******************************************************************
				* 发送数据请求信息
				********************************************************************/
				if(SendOptions!=undefined && SendOptions!=null 
				&& typeof(SendOptions)=="object" && JsonRequest!=undefined 
				&& JsonRequest!=null && typeof(JsonRequest)=="function")
				{
					JsonRequest(SendOptions);	
				}
			}
		};
		/*******************************************************************
		* 重新赋值JSON事件
		********************************************************************/
		if(ThisRefresh!=undefined){ThisRefresh=null;}
		else if(ThisRefresh!=null){ThisRefresh=null;}
		/*******************************************************************
		* 加载网页数据信息
		********************************************************************/
		var options = options || {};
		options['isLock'] = options['isLock'] || false;
		options['isAuto'] = options['isAuto'] || true;
		var contianer = this;
		if(contianer!=undefined && contianer!=null && options!=undefined 
		&& options!=null && options['url']!=undefined 
		&& options['url']!=null && options['url']!='')
		{
			ThisRefresh = new MiniRefresh({
				container:(contianer.id  || '#minirefresh'),
				down: {
					isLock: ((options['isLock']) || false),//是否禁用下拉刷新
					callback: function () {
						try{
							var obj = setTimeout(function(){
								try{GetRequest('down');}catch(err){}	
								try{clearTimeout(obj);}catch(err){}
							},1000);	
						}catch(err){}
					}
				},
				up: {
					isAuto:((options['isAuto']) || true),
					callback: function (){
						if(isUpLoading){
							try{
								var obj = setTimeout(function(){
									try{GetRequest('up');}catch(err){}	
									try{clearTimeout(obj);}catch(err){}
								},1000);	
							}catch(err){}
						}else{
							try{ThisRefresh.endUpLoading(true);}
							catch(err){};	
						}
					}
				}
			});		
		}
	};

})(jQuery);


/******************************************************************************************
*★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★
*发送请求数据主入口函数信息
*★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★★
*******************************************************************************************/
;var JsonRequest = function(options)
{
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
		try{rspJson['text'] = (strMessage || "未知的系统提示信息");}catch(err){}
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
	/*********************************************************************
	*输出错误处理提示信息
	***********************************************************************/
	var doConfirm = function(options)
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
	var JSONResponse = function(rspJson)
	{
		try{
			var rspJson = rspJson || {};
			try{rspJson['type']=rspJson['type'] || 'define';	}catch(err){};
			try{rspJson['success']=rspJson['success'] || 'false';}catch(err){};
			try{rspJson['tips'] = rspJson['tips'] || '来自客户端的反馈发生未知错误,请重试!';}catch(err){};
			try{rspJson['text'] = rspJson['tips'] || '来自客户端的反馈发生未知错误,请重试!';}catch(err){};
		}catch(err){}
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
		else if(rspJson['success']!='true'){PutMessage(options['tips']);return false;}
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
			}
		};
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
	/****************************************************************************************
	*播放数据请求的动画信息
	*****************************************************************************************/
	//try{animation("show");}
	//catch(err){}
	/****************************************************************************************
	*实例化请求对象信息
	*****************************************************************************************/
	var options = options || {};
	options['url'] = options['url'] || '';
	options['type'] = options['type'] || 'get';
	options['dataType'] = options['dataType'] || 'json';
	options['async'] = options['async'] || false;
	/****************************************************************************************
	*处理错误请求数据
	*****************************************************************************************/
	options['error'] = function(err)
	{
		try{
			var timer = setTimeout(function(){
				try{clearTimeout(timer);}catch(err){}
				try{animation("hide");}catch(err){}
			},500);
		}catch(err){}
		try{PutMessage('请求过程中发生错误,请重试！');}
		catch(err){}
		return false;
	};
	options['complete'] = function(err)
	{
		try{
			var timer = setTimeout(function(){
				try{clearTimeout(timer);}catch(err){}
				try{animation("hide");}catch(err){}
			},500);
		}catch(err){}
		
	};
	options['success'] = function(strResponse)
	{
		if(options['dataType']!=undefined && options['dataType']!=null 
		&& options['dataType']=='json' && strResponse!=undefined 
		&& strResponse!=null && typeof(strResponse)=='object')
		{
			try{JSONResponse(strResponse);}
			catch(err){}
		}
		else if(strResponse!=undefined && strResponse!=null 
		&& strResponse!="" && strResponse.match(/{(.*?)}/) 
		&& strResponse.indexOf(":")!=-1)
		{
			try{JSONResponse(strToJson(strResponse));}
			catch(err){alert(err.message);return false;}
		}
		else if(strResponse!=undefined && strResponse!=null 
		&& strResponse!="" && strResponse.match(/[(.*?)]/))
		{
			try{JSONResponse(strToJson(strResponse));}
			catch(err){alert(err.message);return false;}
		}
		else{
			if(options['back']!=undefined 
			&& options['back']!=null 
			&& typeof(options['back'])=='function')
			{
				options['back'](strResponse);
			}	
		}
	};
	/****************************************************************************************
	*验证请求数据的合法性
	*****************************************************************************************/
	if(options==undefined){PutMessage('构建数据请求错误,请重试！');return false;}
	else if(options==null){PutMessage('构建数据请求错误,请重试！');return false;}
	else if(typeof(options)!="object"){PutMessage('构建数据请求错误,请重试！');return false;}
	else if(options["url"]==undefined){PutMessage('构建数据请求错误,请重试！');return false;}
	else if(options["url"]==null){PutMessage('构建数据请求错误,请重试！');return false;}
	else if(options["url"]==""){PutMessage('构建数据请求错误,请重试！');return false;}
	/****************************************************************************************
	*验证ajax对象的合法性
	*****************************************************************************************/
	if(jQuery==undefined){PutMessage('无法获取jQuery对象,请引入jQuery包！');return false;}
	else if(jQuery==null){PutMessage('无法获取jQuery对象,请引入jQuery包！');return false;}
	else if(typeof(jQuery)!="function"){PutMessage('无法获取jQuery对象,请引入jQuery包！');return false;}
	else if(jQuery["ajax"]==undefined){PutMessage('无法获取jQuery对象,请引入jQuery包！');return false;}
	else if(jQuery["ajax"]==null){PutMessage('无法获取jQuery对象,请引入jQuery包！');return false;}
	else if(jQuery["ajax"]==""){PutMessage('无法获取jQuery对象,请引入jQuery包！');return false;}
	/****************************************************************************************
	*开始请求数据信息
	*****************************************************************************************/
	try{jQuery.ajax(options);}
	catch(err){}
};



