# remote-desktop-controller
This project provides a remote desktop controller, able to propagate user interactions (keystrokes, mouse events)  and clipboard across different computers. Actually is not propagated monitor output, in any case this program is useful in order to share mouse/keyboard across PCs.

I've called master the host where is seated the user, while remote controlled PC are slaves.

From the master, the user can move mouse(click,double click,wheel,..) and send keystrokes. Special keys combination are allowed(e.g. Win+..), while Ctrl+Alt+Del is not working (digital signature of .dll is mandatory, in order to intercept this special combination).

The user can switch control from master to slave(or between slaves) throught a GUI or configurable hot-keys. Is also provided the transfer of multi-format clipboards (including recursive folders) between hosts, triggered by special hotkey(by default, Ctrl+Alt+G in order to Get clipboard from active slave,while Ctrl+Alt+S Send clipboard to slave).
The program can start at boot time on slaves.

A TCP server is active on slave hosts, listening over a configurable port. A low latency(nagle=off) TCP connection is used for command and transfer of user interactions(mouse/keyboard), while clicpboard are transfered over a dedicated TCP connection, using the same port.

I want to thanks Marco De Benedictis for his contribution.
