var swfu;
window.onload = function () {
swfu = new SWFUpload({
	// Backend Settings
	upload_url: "SWFupload.aspx?action=start",
	post_params: "",

	// File Upload Settings
	file_size_limit: 1024 * 512, // 2MB
	file_types: "*.*",
	file_types_description: "选择文件",
	file_upload_limit: 0,

	// Event Handler Settings - these functions as defined in Handlers.js
	//  The handlers are not part of SWFUpload but are part of my website and control how
	//  my website reacts to the SWFUpload events.
	swfupload_preload_handler: preLoad,
	swfupload_load_failed_handler: loadFailed,
	file_queue_error_handler: fileQueueError,
	file_dialog_complete_handler: fileDialogComplete,
	upload_start_handler: uploadStart,
	upload_progress_handler: uploadProgress,
	upload_error_handler: uploadError,
	upload_success_handler: uploadSuccess,
	upload_complete_handler: uploadComplete,

	// Button Settings
	//button_image_url : "../plus/swfupload/images/SmallSpyGlassWithTransperancy_17x18d.png",
	button_placeholder_id: "spanButtonPlaceholder",
	button_width: 75,
	button_height: 28,
	button_text: '<span class="button">本地上传</span>',
	button_text_style: '.button {background:url(Images/picBnt.png) no-repeat left top;width:75px;height:28px; line-height:28px; text-align:center;color:#FFFFFF; font-weight:900;} ',
	button_text_top_padding: 4,
	button_text_left_padding: 0,
	button_window_mode: SWFUpload.WINDOW_MODE.TRANSPARENT,
	button_cursor: SWFUpload.CURSOR.HAND,

	// Flash Settings
	flash_url: "swfupload.swf",
	flash9_url: "swfupload_FP9.swf",

	custom_settings: {
		upload_target: "divFileProgressContainer"
	},
	// Debug Settings
	debug: false
});
};