mergeInto(LibraryManager.library, {
  CheckWalletNFTs: function (walletAddress) {
    try {
      window.dispatchReactUnityEvent("CheckWalletNFTs", UTF8ToString(walletAddress));
    } catch (e) {
      console.warn("Failed to dispatch event");
    }
  },
});
