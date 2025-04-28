#  ICB0009-UF3-PR01 - BRIDGE TRAFFIC SIMULATOR
by Alice Aliaga Moreno

## 📋 Project Overview
In this project we will simulate a 100km bidirectional road, which will be single-laned between kilometers 30 to 50.

**Rules**:
- Only one vehicle can cross at a time, independently of direction
- If a vehicle is crossing, all the rest from both directions must wait
- Vehicles going in the oposite direction will be put on priority queue (notified through "Waiting (direction)")

All clients must see the rest of vehicles on the road.

Vehicles will drive the 100km at a speed provided by the server.

