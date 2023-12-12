// Connection Sequence:
// 1. Web app starts Unity in an iframe and listens for events.
// 2. Messages are exchanged with merciv-unity-to-js and merciv-js-to-unity

const MercivJavaScriptBridge = {
    MercivJavaScriptBridgeCreate: function(messageCb) {
        window.addEventListener('merciv-js-to-unity', (e) => {
            const bufferSize = lengthBytesUTF8(e.detail) + 1;
            const buffer = _malloc(bufferSize);
            stringToUTF8(e.detail, buffer, bufferSize);

            try {
                Module.dynCall_vi(messageCb, buffer);
            } finally {
                _free(buffer);
            }
        });
    },

    MercivJavaScriptBridgeSend: function(data) {
        window.dispatchEvent(new CustomEvent('merciv-unity-to-js', { detail: UTF8ToString(data) }));
    }
}

mergeInto(LibraryManager.library, MercivJavaScriptBridge);