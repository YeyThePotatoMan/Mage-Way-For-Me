using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StringInputController : MonoBehaviour
{
  // True if the input is valid, false otherwise.
  public bool isValid = true;
  // List of validators to check the input value when the user changes it.
  public List<Func<string, bool>> valueChangeValidators = new();
  // List of validators to check the input value when the user submits it.
  public List<Func<string, bool>> submitValidators = new();

  // The text input field component.
  [SerializeField]
  private TMP_InputField _textField;    

  private string _previousSubmittedText;
  private string _previousInputText;

  public virtual void Start()
  {
    _textField.onSubmit.AddListener(OnSumbit);
    _textField.onSelect.AddListener(OnSelect);
    _textField.onValueChanged.AddListener(OnValueChanged);
  }

  // Event Handlers //
  private void OnSelect(string value)
  {
    _previousSubmittedText = value;
    _previousInputText = value;
  }

  private void OnSumbit(string value)
  {
    if (!isValid) _textField.text = _previousSubmittedText;
    else
    {
      isValid = true;
      foreach (var action in submitValidators)
      {
        if (action(value))
        {
          isValid = false;
          break;
        }
      }

      if (!isValid) _textField.text = _previousSubmittedText;
    }
  }

  private void OnValueChanged(string value)
  {
    isValid = true;
    foreach (var action in valueChangeValidators)
    {
      if (action(value))
      {
        isValid = false;
        break;
      }
    }

    if (!isValid) _textField.text = _previousInputText;
    else _previousInputText = _textField.text;
  }
}
