using NetworkLib.Packets;

namespace NetworkLib.Networking.StateObjects
{
    public class ClientStateObject : BaseClientStateObject
    {
        public ClientStateObject()
        {
            StateType = StateObjectType.Client;
        }
    }
}
