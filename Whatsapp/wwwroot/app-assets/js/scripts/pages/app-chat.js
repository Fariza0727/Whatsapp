/*=========================================================================================
    File Name: app-chat.js
    Description: Chat app js
    ----------------------------------------------------------------------------------------
    Item Name: Vuexy  - Vuejs, HTML & Laravel Admin Dashboard Template
    Author: PIXINVENT
    Author URL: http://www.themeforest.net/user/pixinvent
==========================================================================================*/

'use strict';
var sidebarToggle = $('.sidebar-toggle'),
  overlay = $('.body-content-overlay'),
  sidebarContent = $('.sidebar-content');

// Window Resize
$(window).on('resize', function () {
  sidebarToggleFunction();
  if ($(window).width() > 992) {
    if ($('.chat-application .body-content-overlay').hasClass('show')) {
      $('.app-content .sidebar-left').removeClass('show');
      $('.chat-application .body-content-overlay').removeClass('show');
    }
  }

  // Chat sidebar toggle
  if ($(window).width() < 991) {
    if (
      !$('.chat-application .chat-profile-sidebar').hasClass('show') ||
      !$('.chat-application .sidebar-content').hasClass('show')
    ) {
      $('.sidebar-content').removeClass('show');
      $('.body-content-overlay').removeClass('show');
    }
  }
});

// Add message to chat - function call on form submit
function enterChat(source) {
  var message = $('.message').val();
  if (/\S/.test(message)) {
    var html = '<div class="chat-content">' + '<p>' + message + '</p>' + '</div>';
    $('.chat:last-child .chat-body').append(html);
    $('.message').val('');
    $('.user-chats').scrollTop($('.user-chats > .chats').height());
  }
}
