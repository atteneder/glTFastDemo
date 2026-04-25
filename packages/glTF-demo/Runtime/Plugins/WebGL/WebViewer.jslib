mergeInto(LibraryManager.library, {
  OnModelLoaded: function (success) {
    if(window.viewer.onModelLoaded) {
      window.viewer.onModelLoaded(success);
    }
  },

  StopWatchStateUpdate: function (duration, frameCount, averageFrameTime, maxFrameTime, minFrameTime) {
    if(window.viewer.updateStopWatch) {
      window.viewer.updateStopWatch(duration, frameCount, averageFrameTime, maxFrameTime, minFrameTime);
    }
  },
});
