using System;

public class NumberInputController : StringInputController
{
    // True if the input is a float, false if it is an integer.
    public bool isFloat = false;

    public override void Start()
    {
        valueChangeValidators.Add(NumberValidator);
        submitValidators.Add(NumberValidator);
        base.Start();
    }

    private bool NumberValidator(string value)
    {
        try
        {
            if (isFloat) float.Parse(value);
            else int.Parse(value);
            return true;
        }
        catch (FormatException) { return false; }
    }
}
