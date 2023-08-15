import React, { Fragment, useCallback, useEffect } from "react";
import { Unity, useUnityContext } from "react-unity-webgl";
import { QueryMetaplex } from "../utilities/metaplex-interface";

const UnityContextHolder = (props) => {
  console.log("Wallet Address from unity component:", props.publicKey);
  const {
    unityProvider,
    sendMessage,
    isLoaded,
    loadingProgression,
    addEventListener,
    removeEventListener,
  } = useUnityContext({
    loaderUrl: "buildUnity/WebGL_Builds.loader.js",
    dataUrl: "buildUnity/WebGL_Builds.data",
    frameworkUrl: "buildUnity/WebGL_Builds.framework.js",
    codeUrl: "buildUnity/WebGL_Builds.wasm",
  });

  const injectWalletAddress = (walletAddress) => {
    console.log("Attempting to send message");
    var shortenedWalletAddress = walletAddress;
    sendMessage("WalletAddress", "SetWalletAddress", shortenedWalletAddress);
  };

  const checkWalletNFTs = useCallback(
    (walletAddressFromUnity) => {
      try {
        QueryMetaplex(walletAddressFromUnity).then((data) => {
          const weaponListString = data.join("||");
          console.log("Weapons to enable: ", weaponListString);
          sendMessage(
            "WeaponSelectionScreen",
            "EnableWeapon",
            weaponListString
          );
        });
      } catch (e) {
        console.log("Error getting weapons in wallet: ", e);
      }
    },
    [sendMessage]
  );

  useEffect(() => {
    addEventListener("CheckWalletNFTs", checkWalletNFTs);
    return () => {
      removeEventListener("CheckWalletNFTs", checkWalletNFTs);
    };
  }, [addEventListener, removeEventListener, checkWalletNFTs]);

  const scaleFactor = 0.65;

  return (
    <div>
      {!isLoaded && (
        <div>
          <p>Loading.. {Math.round(loadingProgression * 100)}% done </p>
        </div>
      )}
      <Fragment>
        <Unity
          unityProvider={unityProvider}
          style={{ width: 1920 * scaleFactor, height: 1080 * scaleFactor }}
        />
        <button onClick={injectWalletAddress(props.publicKey)} hidden={true}>
          Inject!
        </button>
      </Fragment>
    </div>
  );
};

export default UnityContextHolder;
