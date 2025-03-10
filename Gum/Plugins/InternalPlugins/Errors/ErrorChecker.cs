﻿using Gum.DataTypes;
using Gum.DataTypes.Behaviors;
using Gum.DataTypes.Variables;
using Gum.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Xceed.Wpf.AvalonDock.Themes;

namespace Gum.Plugins.Errors;

public class ErrorChecker
{
    public ErrorViewModel[] GetErrorsFor(ElementSave element, GumProjectSave project)
    {
        var list = new List<ErrorViewModel>();

        if(element != null)
        {
            var asComponent = element as ComponentSave;

            list.AddRange(GetBehaviorErrorsFor(asComponent, project));
            list.AddRange(GetParentErrorsFor(element));
        }

        return list.ToArray();
    }

    #region Behavior Errors

    private List<ErrorViewModel> GetBehaviorErrorsFor(ComponentSave component, GumProjectSave project)
    {
        List<ErrorViewModel> toReturn = new List<ErrorViewModel>();

        if(component != null)
        {
            foreach(var behaviorReference in component.Behaviors)
            {
                var behavior = project.Behaviors.FirstOrDefault(item => item.Name == behaviorReference.BehaviorName);

                if(behavior == null)
                {
                    toReturn.Add(new ErrorViewModel
                    {
                        Message = $"Missing reference to behavior {behaviorReference.BehaviorName}"
                    });
                }
                else
                {
                    AddBehaviorErrors(component, toReturn, behavior);
                }
            }
        }

        return toReturn;
    }

    private static void AddBehaviorErrors(ComponentSave component, List<ErrorViewModel> errorList, DataTypes.Behaviors.BehaviorSave behavior)
    {
        foreach (var behaviorInstance in behavior.RequiredInstances)
        {
            AddErrorsForBehaviorInstance(component, errorList, behavior, behaviorInstance);
        }

        // January 23, 2025
        // Vic says:
        // Not sure if we even support veraible lists in behaviors at this point, so
        // let's just worry about behaviors
        foreach(var behaviorVariable in behavior.RequiredVariables.Variables)
        {
            AddErrorsForBehaviorVariable(component, errorList, behavior, behaviorVariable);
        }
    }

    private static void AddErrorsForBehaviorVariable(ComponentSave component, List<ErrorViewModel> toReturn, BehaviorSave behavior, VariableSave behaviorVariable)
    {
        var rfv = new RecursiveVariableFinder(component.DefaultState);
        var variable = rfv.GetVariable(behaviorVariable.Name);

        if(variable == null)
        {

            toReturn.Add(new ErrorViewModel
            {
                Message = $"The behavior {behavior} " +
                        $"requires a variable named {behaviorVariable.Name} but this variable doesn't exist. " +
                        $"Add a custom variable or expose a variable and give it the required name to solve this error."
            });
        }
        else if(variable.Type != behaviorVariable.Type)
        {
            toReturn.Add(new ErrorViewModel
            {
                Message = $"The behavior {behavior} " +
                        $"requires a variable named {behaviorVariable.Name} with type {behaviorVariable.Type}. " +
                        $"This variable exists but it has the wrong type {variable.Type};"
            });
        }

        // no errors
    }

    private static void AddErrorsForBehaviorInstance(ComponentSave component, List<ErrorViewModel> toReturn, BehaviorSave behavior, BehaviorInstanceSave behaviorInstance)
    {
        var candidateInstances = component.Instances.Where(item => item.Name == behaviorInstance.Name).ToList();
        if (!string.IsNullOrEmpty(behaviorInstance.BaseType))
        {
            candidateInstances = candidateInstances.Where(item => item.IsOfType(behaviorInstance.BaseType)).ToList();
        }

        if (behaviorInstance.Behaviors.Any())
        {
            var requiredBehaviorNames = behaviorInstance.Behaviors.Select(item => item.Name);
            candidateInstances = candidateInstances.Where(item =>
            {
                bool fulfillsRequirements = false;
                var element = ObjectFinder.Self.GetComponent(item.BaseType);
                if (element != null)
                {
                    var implementedBehaviorNames = element.Behaviors.Select(implementedBehavior => implementedBehavior.BehaviorName);

                    fulfillsRequirements = requiredBehaviorNames.All(required => implementedBehaviorNames.Contains(required));

                }
                return fulfillsRequirements;
            }).ToList();

        }

        if (!candidateInstances.Any())
        {
            string message = $"Missing instance with name {behaviorInstance.Name}";
            if (!string.IsNullOrEmpty(behaviorInstance.BaseType))
            {
                message += $" of type {behaviorInstance.BaseType}";
            }
            if (behaviorInstance.Behaviors.Any())
            {
                if (behaviorInstance.Behaviors.Count == 1)
                {
                    message += " with behavior type ";
                }
                else
                {
                    message += " with behavior types ";
                }
                var behaviorsJoined = string.Join(", ", behaviorInstance.Behaviors.Select(item => item.Name).ToArray());
                message += behaviorsJoined;
            }

            message += $" needed by behavior {behavior.Name}";

            toReturn.Add(new ErrorViewModel
            {
                Message = message
            });
        }
    }

    #endregion

    #region Parent Errors

    List<ErrorViewModel> GetParentErrorsFor(ElementSave elementSave)
    {
        var toReturn = new List<ErrorViewModel>();
        // Do we want to use the RecursiveVariableFinder
        // to report parenting errors recursively? I vote "no"
        // because if we report an error in a derived element or
        // state, the user may fix the parent error there, when it 
        // really should be fixed in the base.
        foreach(var state in  elementSave.AllStates)
        {
            foreach(var variable in state.Variables)
            {
                if (!string.IsNullOrEmpty(variable.SourceObject) && variable.GetRootName() == "Parent")
                {
                    var value = variable.Value as string;

                    if(!string.IsNullOrEmpty(value))
                    {
                        var instance = elementSave.GetInstance(value);

                        if(instance == null)
                        {
                            var error = new ErrorViewModel();
                            error.Message = $"{variable.SourceObject} has a parent set to {value} which does not exist in the state {state.Name}";
                            toReturn.Add(error);
                        }
                    }
                }
            }
        }

        return toReturn;
    }

    #endregion
}
