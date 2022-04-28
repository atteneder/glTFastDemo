﻿var ImageDownloaderPlugin = {
  ImageDownloader: function(str, fn, mimeType, noAutoClick) {
      var msg = UTF8ToString(str);
      var fname = UTF8ToString(fn);
      var contentType = mimeType;
      function fixBinary (bin)
      {
        var length = bin.length;
        var buf = new ArrayBuffer(length);
        var arr = new Uint8Array(buf);
        for (var i = 0; i < length; i++)
        {
              arr[i] = bin.charCodeAt(i);
        }
        return buf;
      }
      var binary = fixBinary(atob(msg));
      var data = new Blob([binary], {type: contentType});
      var link = document.createElement('a');
    link.download = fname;
    link.innerHTML = 'DownloadFile';
    link.setAttribute('id', 'ImageDownloaderLink');
    if(window.webkitURL != null)
    {
        link.href = window.webkitURL.createObjectURL(data);
    }
    else
    {
        link.href = window.URL.createObjectURL(data);
        link.onclick = function()
        {
            var child = document.getElementById('ImageDownloaderLink');
            child.parentNode.removeChild(child);
        };
        link.style.display = 'none';
        document.body.appendChild(link);
    }
	
	if(noAutoClick)
	{
		console.log("link has been added for: " + data);
		if(mimeType == "image/png" && CompositionResearchToolAPI != null)
      CompositionResearchToolAPI.ImageDataHandler(data);
    if(mimeType == "model/gltf-binary" && CompositionResearchToolAPI != null)
      CompositionResearchToolAPI.GLBDataHandler(data);
	}
	else {
		link.click();
	}
  }
};
mergeInto(LibraryManager.library, ImageDownloaderPlugin);