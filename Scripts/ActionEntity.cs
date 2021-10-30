using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public abstract class ActionEntity : Entity {

	public ActionEntity (string name, int x, int y, bool solid) : base(name, x, y, solid) {
		
	}

	public ActionEntity (ActionEntity from, WorldState newWorld) : base(from, newWorld) {
		
	}

	public abstract void Do (Agent agent, Action action,
	[CallerFilePath] string callerFilePath = "", 
	[CallerLineNumber] long callerLineNumber = 0,
	[CallerMemberName] string callerMember= "");
	
	public abstract List<string> GetActionNames();
}
