using System;

namespace ET
{
	public  class DlgTest :Entity,IAwake,IUILogic
	{

		public DlgTestViewComponent View { get => this.Parent.GetComponent<DlgTestViewComponent>();}
    }
}
