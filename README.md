# remote-desktop-controller
This project provides a remote desktop controller, able to propagate user interactions (keystrokes, mouse events)  and clipboard across different computers. Actually is not propagated monitor output, in any case this program is useful in order to share mouse/keyboard across PCs.

There are two components: master and slave.
The first act as a client, in order to control remote PCs. 
Slaves are launched on controlled computers, they can run from boot.

The user can switch across slaves(or come back to local master) using configurable hotkeys combination.
Is possible to move clipboard between slave and master(or vice versa) using Ctrl+Alt+C and Ctrl+Alt+V.
Any format of clipboard is supported(also recursive folders)
