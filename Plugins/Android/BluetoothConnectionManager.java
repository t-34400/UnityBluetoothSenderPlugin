package com.example.bluetoothapp;

import android.app.Activity;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothManager;
import android.bluetooth.BluetoothSocket;
import android.content.Context;
import android.os.Build;
import android.os.Handler;
import android.os.Message;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.lang.ref.WeakReference;
import java.util.HashSet;
import java.util.Set;

public class BluetoothConnectionManager {
    private final WeakReference<Activity> activityWeakReference;

    private final BluetoothAdapter bluetoothAdapter;
    private ConnectThread connectThread;
    private ConnectedThread connectedThread;

    public BluetoothConnectionManager(Activity activity) {
        activityWeakReference = new WeakReference<>(activity);

        bluetoothAdapter = getBluetoothAdapter(activity);
        BluetoothPermissionManager.requestBluetoothPermissionsIfNeeded(activity);
        BluetoothPermissionManager.enableBluetoothIfNeeded(activity, bluetoothAdapter);
    }

    public boolean isActivityAlive() {
        return activityWeakReference.get() != null;
    }

    public boolean isEnabled() {
        return isActivityAlive() && bluetoothAdapter != null && bluetoothAdapter.isEnabled();
    }

    public boolean hasBluetoothPermissions()
    {
        Activity activity = activityWeakReference.get();
        return activity != null && BluetoothPermissionManager.hasBluetoothPermissions(activity);
    }

    public boolean isConnecting() { return connectThread != null; }
    public boolean isConnected() { return connectedThread != null; }

    public Set<BluetoothDevice> getPairedDevices() {
        Activity activity = activityWeakReference.get();

        if (activity == null || bluetoothAdapter == null) {
            return new HashSet<>();
        }

        BluetoothPermissionManager.enableBluetoothIfNeeded(activity, bluetoothAdapter);
        @SuppressWarnings("MissingPermission")
        Set<BluetoothDevice> pairedDevices = bluetoothAdapter.getBondedDevices();

        return pairedDevices;
    }

    public void connectToDevice(BluetoothDevice bluetoothDevice) {
        if (connectedThread != null && connectedThread.isAlive()) {
            connectedThread.cancel();
            connectedThread = null;
        }

        if (bluetoothAdapter == null) {
            return;
        }
        connectThread = new ConnectThread(bluetoothAdapter, bluetoothDevice, this::manageConnectedSocket);
        connectThread.start();
    }

    public void send(byte[] data) {
        if (connectedThread != null) {
            connectedThread.write(data);
        }
    }

    public void close() {
        if (connectThread != null) {
            connectThread.cancel();
            connectThread = null;
        }
        if (connectedThread != null) {
            connectedThread.cancel();
            connectedThread = null;
        }
    }

    private void manageConnectedSocket(BluetoothSocket socket) {
        connectThread = null;

        if (connectedThread != null && connectedThread.isAlive()) {
            connectedThread.cancel();
            connectedThread = null;
        }

        Activity activity = activityWeakReference.get();
        if (activity == null) {
            return;
        }
        Handler handler = new Handler(activity.getMainLooper(), this::handleMessage);

        connectedThread = new ConnectedThread(socket, handler);
        connectedThread.start();
    }

    private boolean handleMessage(Message message) {
        // Handle received message
        return true;
    }

    private static BluetoothAdapter getBluetoothAdapter(Context context) {
        int version = Build.VERSION.SDK_INT;

        if (version >= 23) {
            BluetoothManager bluetoothManager = context.getSystemService(BluetoothManager.class);
            return bluetoothManager.getAdapter();
        } else if (version >= 18) {
            BluetoothManager bluetoothManager = (BluetoothManager) context.getSystemService(Context.BLUETOOTH_SERVICE);
            return bluetoothManager.getAdapter();
        }
        return BluetoothAdapter.getDefaultAdapter();
    }
}
