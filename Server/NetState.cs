﻿using System.Collections;
using System.Net.Sockets;
using Cedserver;

namespace Server; 

public class NetState {
    public Socket Socket { get; }
    public MemoryStream SendQueue { get; }
    public MemoryStream ReceiveQueue { get; }
    public Account? Account { get; set; }
    public ArrayList Subscriptions { get; } //TODO: Fill in correct element type
    public DateTime LastAction { get; set; }

    public NetState(Socket socket) {
        Socket = socket;
        SendQueue = new MemoryStream();
        ReceiveQueue = new MemoryStream();
        Account = null;
        Subscriptions = new ArrayList();
        LastAction = DateTime.Now;
    }
}