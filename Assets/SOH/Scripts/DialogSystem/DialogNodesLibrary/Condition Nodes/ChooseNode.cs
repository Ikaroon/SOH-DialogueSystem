using UnityEngine;
using SpyOnHuman.DialogSystem.NodeFramework;

namespace SpyOnHuman.DialogSystem
{
    [NodeData("Choose", "This node will choose one node randomly", 64f, 96f, true, 1f, 0.9724138f, 0f)]
    public class ChooseNode : Node
    {
	
        #region Handles
		
		// Input Handles
		[NodeHandle(0, ConnectionType.Input, 0f, true, "")]
		public NodeConnection input0;


		
		// Output Handles
		[NodeHandle(0, ConnectionType.Output, 0f, true, "")]
		public NodeConnection output0;

		[NodeHandle(1, ConnectionType.Output, 32f, true, "")]
		public NodeConnection output1;



        #endregion

        #region Update Methods

        public override Node PrepareNode()
        {
            int outputID = (int)Random.Range(0, 1);
            switch(outputID)
            {
                case 0:
                    {
                        if (output0 != null)
                        {
                            return output0.to;
                        }
                        else if (output1 != null)
                        {
                            return output1.to;
                        }
                    }
                    break;
                case 1:
                    {
                        if (output1 != null)
                        {
                            return output1.to;
                        }
                        else if (output0 != null)
                        {
                            return output0.to;
                        }
                    }
                    break;

            }
            return null;
        }


        #endregion

    }
}
