
# Telesurgery-HMD
This is the repo for the telesurgery project that runs on the HMD, as of 04/13/2022. 

## Installation

1. Clone the repo
   ```sh
   git clone https://github.com/ehsan-azimi/Telesurgery-HMD.git
   ```
2. Open an example scene
   ```
   .\Assets/Scenes/Ball Joint_Ho2.unity
   ```
3. Enjoy!
## Dependencies
* Unity 

## File to open
* `Assets/Scenes/Ball Joint_Ho2.unity` if you want to run the IROS script

## Scripts description
>All scripts are similar to the Unity project. However, since OpenZen does not run on UWP, the joint data is sent through UDP network. HMD is the client and receives the data. On the sender side (Unity program on the computer) Set the IP as the IP of the HoloLens. The port number must be the same on both server and the client.

* **`UDPCommunication.cs`**
>UDP client function to receive joint information on the HMD from the Unity.
