package com.example.bluetoothapp;

import android.app.Activity;
import android.bluetooth.BluetoothDevice;
import android.util.Log;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.Set;

public class BluetoothConnectionManagerUnityWrapper {
    private final BluetoothConnectionManager bluetoothConnectionManager;

    public BluetoothConnectionManagerUnityWrapper(Activity activity) {
        bluetoothConnectionManager = new BluetoothConnectionManager(activity);
    }

    public static BluetoothConnectionManagerUnityWrapper getInstance(Activity activity) {
        return new BluetoothConnectionManagerUnityWrapper(activity);
    }

    public boolean isActivityAlive() { return bluetoothConnectionManager.isActivityAlive(); }

    public int getBluetoothStatus() {
        if (!bluetoothConnectionManager.isActivityAlive()) {
            return -1;
        }
        if (!bluetoothConnectionManager.isEnabled()) {
            return -2;
        }
        if (!bluetoothConnectionManager.hasBluetoothPermissions()) {
            return -3;
        }

        return 0;
    }

    public int getConnectionStatus() {
        if (bluetoothConnectionManager.isConnected()) {
            return 2;
        }
        if (bluetoothConnectionManager.isConnecting()) {
            return 1;
        }

        return 0;
    }

    public String getPairedDeviceJson() {
        return convertToJsonString(bluetoothConnectionManager.getPairedDevices());
    }

    public void connectToDeviceByAddress(String address) {
        Log.d("Bluetooth", "Connect to address: " + address);
        
        Set<BluetoothDevice> devices = bluetoothConnectionManager.getPairedDevices();
        for (BluetoothDevice device: devices) {
            if (device.getAddress().equals(address)) {
                Log.d("Bluetooth", "Connect to " + device.getName());
                bluetoothConnectionManager.connectToDevice(device);
            }
        }
    }

    public void send(byte[] data) { bluetoothConnectionManager.send(data); }
    public void close() { bluetoothConnectionManager.close(); }

    @SuppressWarnings("MissingPermission")
    public static String convertToJsonString(Set<BluetoothDevice> bluetoothDevices) {
        JSONArray jsonArray = new JSONArray();

        for (BluetoothDevice device : bluetoothDevices) {
            try {
                JSONObject deviceJson = new JSONObject();
                deviceJson.put("name", device.getName());
                deviceJson.put("address", device.getAddress());
                jsonArray.put(deviceJson);
            } catch (JSONException e) {
                e.printStackTrace();
            }
        }

        return "{ \"devices\": " + jsonArray.toString() + " }";
    }
}
