//import Header from "../components/Header";
import UnityContextHolder from "../components/UnityContextHolder";
import { useEffect, useState } from "react";
import {
  MainContainer,
  ConnectButton,
} from "../components/styles/GameLoginPage.styled";

const GameLoginPage = () => {
  const [walletAddress, setWalletAddress] = useState(null);

  const checkIfWalletIsConnected = async () => {
    try {
      const { solana } = window;

      if (solana) {
        if (solana.isPhantom) {
          console.log("Phantom wallet found!");
          const response = await solana.connect({ onlyIfTrusted: true });
          console.log(
            "Connected with Public Key:",
            response.publicKey.toString()
          );
          setWalletAddress(response.publicKey.toString());
        }
      } else {
        alert("Solana object not found! Get a Phantom Wallet 👻");
      }
    } catch (error) {
      console.error(error);
    }
  };

  const connectWallet = async () => {
    const { solana } = window;

    if (solana) {
      const response = await solana.connect();
      console.log("Connected with Public Key:", response.publicKey.toString());
      setWalletAddress(response.publicKey.toString());
    }
  };

  const disconnectWallet = async () => {
    const { solana } = window;

    if (solana) {
      await solana.disconnect();
      console.log("Disconnected from wallet");
      setWalletAddress(null);
    }
  };

  const renderConnectButtonContainer = () => (
    <ConnectButton onClick={connectWallet}>Connect Wallet</ConnectButton>
  );

  const renderDisonnectButtonContainer = () => (
    <ConnectButton onClick={disconnectWallet}>Disconnect</ConnectButton>
  );

  useEffect(() => {
    const onLoad = async () => {
      await checkIfWalletIsConnected();
    };
    window.addEventListener("load", onLoad);
    return () => window.removeEventListener("load", onLoad);
  }, []);

  return (
    <MainContainer>
      {!walletAddress && renderConnectButtonContainer()}
      {walletAddress && renderDisonnectButtonContainer()}
      {walletAddress && <UnityContextHolder publicKey={walletAddress} />}
    </MainContainer>
  );
};

export default GameLoginPage;
