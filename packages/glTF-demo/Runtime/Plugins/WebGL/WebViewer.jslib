mergeInto(LibraryManager.library, {
  OnModelLoaded: function (success) {
    if(window.viewer.onModelLoaded) {
      window.viewer.onModelLoaded(success);
    }
  }
});
