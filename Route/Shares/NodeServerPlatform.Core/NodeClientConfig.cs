using Log;

namespace NodeServerPlatform.Core
{
    public class NodeClientConfig
    {
        public NodeClientConfig(ILog log, INodeSharePacketTable packetTab, INodeShareHandleTable handleTab)
        {
            Log = log;
            PacketTab = packetTab;
            HandleTab = handleTab;
        }

        public ILog Log { get; }
        public INodeSharePacketTable PacketTab { get; set; }
        public INodeShareHandleTable HandleTab { get; set; }
        public string Ip { get; set; }
        public int Port { get; set; }
        public string Name { get; set; }
        public bool ReConnect { get; set; }
    }
}