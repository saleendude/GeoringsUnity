# GeoringsUnity

## Georings: The Game

Georings is a multiplayer 3D RPG game built on Unity, leveraging NFTs on Solana to be used as in-game items. The game was built to run in browser on WebGL and used Mirror's Websockets transport to enable multiplayer gameplay that was tested with 25 concurrent players per session.

## Why it was built on Solana
Initially starting off with the idea of building cross-chain tooling that NFT projects could use for rapid prototyping, it wasn’t until we started developing our in-house gaming projects did we realize that most solutions offered by Ethereum simply weren’t scalable enough for a fast-paced game.

While there are other chains that offer quick and cheap transactions, none of them come with the robustness and community support that Solana provides. This is apart from the constant state of innovation the chain finds itself in, with projects like Shadow Drive, Metaplex and now more recently SHIFT & Backpack moving the game forward for NFT-based games like Georings.

## How Georings was built
Georings started off as an idea to tackle one of the main issues in NFT space: bot spam during a mint. Each Georing minted would be tied to the minter’s location in world space. This minting mechanism would result in bots minting a ton of pieces that have the same set of attributes, hence diluting the value of their minted pieces by dropping their rarity in the collection.

It was decided to elaborate on this idea by developing an entire game around these minted Georings, while still adhering to the core concepts of Web3. From the get-go, there were 3 requirements:
- Must be a fun to play multiplayer title.
- Must be Web3 authenticated, for logins (wallet address) and game sub-systems (NFTs).
- Must be browser rendered to attain max availability for new Web3 entrants.

![image](https://github.com/saleendude/GeoringsUnity/assets/35657745/6e7e03d7-5669-46c7-84eb-7000e163ab5b)


## Screenshots and Gameplay POC

![Homepage](/screenshots/1.jpg "Homepage")
![Homepage](/screenshots/2.jpg "Homepage")
![Homepage](/screenshots/3.jpg "Homepage")

__Proof of Concept video__:

> [!NOTE]
> Inline video playback disabled due to file size

https://github.com/saleendude/GeoringsUnity/blob/main/screenshots/Gameplay.mp4

## Tech stack

All three of these technical feats were achieved, without raising a single dollar during development. 
A breakdown of the tech stack behind the Georings game:
- Base game built on Unity. Uses Mirror Networking for the multiplayer aspect.
- Solana/Web3.js for login authentication and Azure playfab for secondary save-states.
- Metaplex CandyMachine V2 API for validation of NFTs in wallet.
- Metaplex Sugar for ad-hoc mints when players request it.
- React JS for the frontend, served by an Apache server configured for WebGL file distribution via Gzip/Brotli compression.
- Microsoft Playfab for load balancing and sharding player sessions.

![image](https://github.com/saleendude/GeoringsUnity/assets/35657745/eb9d78b4-c5a3-4d35-858c-a70029748401)

--- 
