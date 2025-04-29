# 🚦 ICB0009-UF3-PR01 EXERCISE 2 - BRIDGE TRAFFIC SIMULATOR
by Alice Aliaga Moreno

## 📋 Project Overview
The goal of this exercise is to simulate a road where multiple vehicles travel in opposite directions and communicate with a server. The server must manage the global state of the road and send updates to all connected clients. The clients represent the vehicles and update their position as they move, sending the corresponding data to the server.

This exercise is divided into several stages that allow us to implement the simulation and communication between clients and the server step by step.

## 💡The Solution
The project consists of the following components:

- **Server**: Accepts connections from multiple clients, manages the road simulation, and sends updates to all connected clients.

- **Client**: Represents a vehicle that connects to the server, sends its position, and receives road updates from the server.

- **NetworkStreamClass**: An auxiliary class that manages reading and writing data between the client and server using NetworkStream.

- **Vehicle** and **Carretera** Classes: Define the structure of the vehicles and the road in the simulation.


## 🧮 Explanation by Steps
### 📍 Stage 1: Initial Setup and Network Handshake
The client initialises the connection to the server. Then both exchange initialisation messages to establish the connection.

**Client Side:**

- The client sends an "INIT" message to start the handshake (`NetworkStreamClass.EscribirMensajeNetworkStream`).

- The server responds with a message containing the vehicle’s ID and bearing, which the client then acknowledges and sends back.

**Server Side:**

- The server listens for "INIT" and respons with the vehicle's ID and bearing
- Upon acknowledgement, the server completes the handshake

### 📍 Stage 2: Create and Add Vehicle (Vehiculo)
After the handshake, the client creates a `Vehiculo` object and sends it to the server. The server will add it to the `Carretera`.

**Client Side:**

- The client creates a new `Vehiculo` object with an ID and direction (from the handshake).

- The vehicle is then sent to the server using `NetworkStreamClass.EscribirDatosVehiculoNS`

**Server Side:**

- The server receives the `Vehiculo` object and adds it to the `Carretera` using `carretera.AñadirVehiculo(vehiculo)`.

### 📍 Stage 3: Update Vehicle Data
The client periodically updates the vehicle’s position and speed, which is sent to the server. The server updates the vehicle’s data and notifies all clients.

**Client Side:**

- The client simulates the movement of the vehicle by updating its position and sending the updates to the server.

- If the vehicle reaches its destination (Pos == 100), the client marks it as finished `(vehiculo.Acabado = true)` and sends the update.

**Server Side:**
- The server receives the updated vehicle data and updates the corresponding `Vehiculo` object in the `Carretera`.

- The server then sends the updated `Carretera` to all connected clients.

### 📍 Stage 4: Send Carretera Data to Clients
The server continuously sends updates of the Carretera to all connected clients. This is done in a separate method that iterates through all connected clients and sends the serialized Carretera object.

**Server Side:**

- The server serializes the `Carretera` object and sends it to each connected client.

### 📍 Stage 5: Listen for Carretera Data from the Server
The clients will be constantly listening for the server sending `Carretera` data in order to update their known status of the carretera.

**Client Side:**

- The client will unpack the `Carretera` object and show it on screen.