
using System;

// Attribute to mark a class can be inspected in the level editor.
[AttributeUsage(AttributeTargets.Class)]
public class Inspectable : Attribute { }

// Attribute to mark a field as an editable key for the inspector in the level editor.
[AttributeUsage(AttributeTargets.Field)]
public class InspectKey : Attribute
{
    public int key;
    public InspectKey(int key) { this.key = key; }
}

// Attribute to assign validators to a field to check the input value when the user changes it. It is also assigned to Submit events.
[AttributeUsage(AttributeTargets.Field)]
public class InspectChangeValidators : Attribute
{
    public Func<string, bool>[] validators;
    public InspectChangeValidators(params Func<string, bool>[] validators) { this.validators = validators; }
}

// Attribute to assign validators to a field to check the input value when the user submits it.
[AttributeUsage(AttributeTargets.Field)]
public class InspectSubmitValidators : Attribute
{
    public Func<string, bool>[] validators;
    public InspectSubmitValidators(params Func<string, bool>[] validators) { this.validators = validators; }
}