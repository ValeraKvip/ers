mergeInto(LibraryManager.library, {

  Hello: function () {
    window.alert("Hello, world!");
  },


  RequestUserData: function () {
    if (window.unityInstance) {
      window.unityInstance.SendMessage("TelegramController", "SetWebAppUser", JSON.stringify(window.Telegram.WebApp.initDataUnsafe.user));
    }
  },


  RequestThemeParams: function () {
    if (window.unityInstance) {
      window.unityInstance.SendMessage("TelegramController", "SetThemeParams", JSON.stringify(window.Telegram.WebApp.themeParams));
    }
  },


  ShowMainButton: function (text) {
    if (window && window.Telegram && window.Telegram.WebApp) {
      if (text) {
        window.Telegram.WebApp.MainButton.setText(UTF8ToString(text));
      }
      window.Telegram.WebApp.MainButton.show();
    }
  },


  HideMainButton: function () {
    if (window && window.Telegram && window.Telegram.WebApp) {
      window.Telegram.WebApp.MainButton.hide();
    }
  },

  MainButtonShowProgress: function () {
    if (window && window.Telegram && window.Telegram.WebApp) {
      window.Telegram.WebApp.MainButton.showProgress();
    }
  },

  MainButtonHideProgress: function () {
    if (window && window.Telegram && window.Telegram.WebApp) {
      window.Telegram.WebApp.MainButton.hideProgress();
    }
  },


  ShowBackButton: function () {
    if (window && window.Telegram && window.Telegram.WebApp) {
      window.Telegram.WebApp.BackButton.show();
    }
  },


  HideBackButton: function () {
    if (window && window.Telegram && window.Telegram.WebApp) {
      window.Telegram.WebApp.BackButton.hide();
    }
  },

  ShowAlert: function (text) {
    if (window && window.Telegram && window.Telegram.WebApp) {
      window.Telegram.WebApp.BackButton.showAlert(UTF8ToString(text));
    }
  },

  ShowShareJoinCode: function (code) {

  },

  Ready: function () {
    if (window && window.Telegram && window.Telegram.WebApp) {
      window.Telegram.WebApp.ready();
    }
  },

  Close: function () {
    if (window && window.Telegram && window.Telegram.WebApp) {
      window.Telegram.WebApp.close();
    }
  },

  Expand: function () {
    if (window && window.Telegram && window.Telegram.WebApp) {
      window.Telegram.WebApp.expand();
    }
  },


  HapticFeedback: function (level) {
    if (window && window.Telegram && window.Telegram.WebApp) {
      window.Telegram.WebApp.HapticFeedback.notificationOccurred(UTF8ToString(level));
    }
  },

  ShowScanQrPopup: function (text) {
    if (window && window.Telegram && window.Telegram.WebApp) {
      window.Telegram.WebApp.showScanQrPopup({
        text: UTF8ToString(text)
      });
    }
  },

  CloseScanQrPopup: function () {
    if (window && window.Telegram && window.Telegram.WebApp) {
      window.Telegram.WebApp.closeScanQrPopup();
    }
  },



});