using LlamaLibrary.RemoteWindows;

namespace NavigationTest
{
    public class TelepotTown: RemoteWindow<TelepotTown>
    {
        private const string WindowName = "TelepotTown";
        
        public TelepotTown() : base(WindowName)
        {

        }
    }
}