import { Metaplex } from "@metaplex-foundation/js";
import { Connection, PublicKey } from "@solana/web3.js";

export const QueryMetaplex = async (ownerAddress) => {
  // ====== connection variables =======
  const connection = new Connection("https://devnet.genesysgo.net/");
  const metaplex = new Metaplex(connection);

  // ====== wallet variables =======
  const owner = new PublicKey(ownerAddress);

  // ====== Unity-React connection variables ======
  const weaponTypes = {
    Nothing: "nothing",
    SwordWooden: "swordWooden",
    SwordSteel: "swordSteel",
    SwordRuby: "swordRuby",
    SwordEmerald: "swordEmerald",
    Scythe: "sycthe",
    Staff: "staff",
    Hammer: "hammer",
    Dagger: "dagger",
    Axe: "axe",
  };
  Object.freeze(weaponTypes); // needed to create weaponType enum in JS

  const getNftsInWallet = metaplex.nfts().findAllByOwner(owner); // Create metaplex task to find all NFTs in owner wallet (type PublicKey, not string)
  const nftList = await getNftsInWallet.run(); // Run task to get result of findAllByOwner fn.

  var weaponsInWallet = []; // initialise empty array to store weapons if they exist in wallet

  // loop through each NFT in list of NFTs in wallet
  for (const element of nftList) {
    if (element.name === "Georings Beta Weapons") {
      const mint_address = element.mintAddress;
      const NFTDetails = await metaplex.nfts().findByMint(mint_address).run(); // task to find NFT data of current Georing Weapon NFT
      var attributes = NFTDetails.json.attributes[2].value.split(" ").join(""); // selecting 'Weapon Name' attribute from array of attributes, & removing space i.e Sword Wooden => SwordWooden to match enum weaponTypes
      weaponsInWallet.push(attributes); // store weapon in empty array initialized earlier.
    }
  }

  // remove multiple instances of the same weapon in weaponsInWallet array
  weaponsInWallet = [...new Set(weaponsInWallet)];
  var weaponsToEnable = [];

  // loop through all weapons in weaponsInWallet array
  weaponsInWallet.forEach((weaponDetail) => {
    const weaponToEnable = weaponTypes[weaponDetail]; //use weaponTypes enum to find which weapon to enable
    weaponsToEnable.push(weaponToEnable);
  });

  return weaponsToEnable;
};
