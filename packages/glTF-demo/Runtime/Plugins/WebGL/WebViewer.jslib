mergeInto(LibraryManager.library, {
  OnModelLoaded: function () {
    if(window.viewer.onModelLoaded) {
      window.viewer.onModelLoaded();
    }
  }
});
