using System;
using System.Collections.Generic;

public struct Action {
    public string _entityName;
    public string _actionName;
    public List<object> _actionParameters;

    public Action (string entityName, string actionName) {
        _entityName = entityName;
        _actionName = actionName;
        _actionParameters = null;
    }
}   