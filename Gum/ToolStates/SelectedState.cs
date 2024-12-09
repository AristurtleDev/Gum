﻿using System;
using System.Collections.Generic;
using System.Linq;
using Gum.DataTypes;
using Gum.Managers;
using Gum.DataTypes.Variables;
using System.Windows.Forms;
using Gum.Wireframe;
using Gum.Plugins;
using Gum.Debug;
using RenderingLibrary;
using Gum.DataTypes.Behaviors;
using Gum.Controls;

namespace Gum.ToolStates
{
    public class SelectedState : ISelectedState
    {
        #region Fields

        static ISelectedState mSelf;

        VariableSave mSelectedVariableSave;

        StateView stateView;
        public StateView StateView
        {
            get => stateView;
            set => stateView = value;
        }

        #endregion

        #region Properties

        public static ISelectedState Self
        {
            // We usually won't use this in the actual product, but useful for testing
            set
            {
                mSelf = value;
            }
            get
            {
                if (mSelf == null)
                {
                    mSelf = new SelectedState();
                }
                return mSelf;
            }
        }

        TreeNode GetComponentTreeNodeRoot(TreeNode treeNode)
        {
            while (treeNode != null)
            {
                if (treeNode.Tag is ElementSave)
                {
                    return treeNode;
                }
                else if (!treeNode.IsTopElementContainerTreeNode())
                {
                    treeNode = treeNode.Parent;
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        public ScreenSave SelectedScreen
        {
            get
            {
                return GetComponentTreeNodeRoot(ElementTreeViewManager.Self.SelectedNode)
                    ?.Tag as ScreenSave;
            }
            set
            {
                // We don't want this to unset selected components or standards if this is set to null
                if (value != SelectedScreen && (value != null || SelectedScreen == null || SelectedScreen is ScreenSave))
                {
                    ElementTreeViewManager.Self.Select(value);
                }
            }
        }

        public ComponentSave SelectedComponent
        {
            get
            {
                return GetComponentTreeNodeRoot(ElementTreeViewManager.Self.SelectedNode)
                    ?.Tag as ComponentSave;
            }
            set
            {
                if (value != SelectedComponent && (value != null || SelectedComponent == null || SelectedComponent is ComponentSave))
                {
                    ElementTreeViewManager.Self.Select(value);
                }
            }
        }

        public BehaviorSave SelectedBehavior
        {
            get
            {
                TreeNode treeNode = ElementTreeViewManager.Self.SelectedNode;

                if (treeNode != null && treeNode.IsBehaviorTreeNode())
                {
                    return treeNode.Tag as BehaviorSave;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != SelectedBehavior)
                {
                    if (value != null)
                    {
                        ElementTreeViewManager.Self.Select(value);
                    }
                    else if (value == null && SelectedBehavior != null)
                    {
                        ElementTreeViewManager.Self.SelectedNode = null;
                    }
                }
            }
        }

        public IStateContainer SelectedStateContainer
        {
            get
            {
                if(SelectedComponent != null)
                {
                    return SelectedComponent;
                }
                else if(SelectedScreen != null)
                {
                    return SelectedScreen;
                }
                else if(SelectedStandardElement != null)
                {
                    return SelectedStandardElement;
                }
                else if(SelectedBehavior != null)
                {
                    return SelectedBehavior;
                }

                return null;
            }
        }

        public StandardElementSave SelectedStandardElement
        {
            get
            {
                return GetComponentTreeNodeRoot(ElementTreeViewManager.Self.SelectedNode)
                    ?.Tag as StandardElementSave;
            }
            set
            {
                if (value != SelectedStandardElement && (value != null || SelectedStandardElement == null || SelectedStandardElement is StandardElementSave))
                {
                    ElementTreeViewManager.Self.Select(value);
                }
            }
        }

        public ElementSave SelectedElement
        {
            get
            {
                return (ElementSave)SelectedScreen ??
                    (ElementSave)SelectedComponent ??
                    (ElementSave)SelectedStandardElement;
            }
            set
            {
                if (value != SelectedElement)
                {
                    if (value == null)
                    {
                        SelectedScreen = null;
                        SelectedStandardElement = null;
                        SelectedComponent = null;
                    }
                    else if (value is ScreenSave)
                    {
                        SelectedScreen = value as ScreenSave;

                        if(SelectedTreeNode == null && SelectedScreen != null)
                        {
                            // we tried to select something (like through a re-select) that no longer exists:
                            SelectedScreen = null;
                        }
                    }
                    else if (value is ComponentSave)
                    {
                        SelectedComponent = value as ComponentSave;
                        if(SelectedTreeNode == null && SelectedComponent != null)
                        {
                            SelectedComponent = null;
                        }
                    }
                    else if (value is StandardElementSave)
                    {
                        SelectedStandardElement = value as StandardElementSave;
                        if(SelectedTreeNode == null && SelectedStandardElement != null)
                        {
                            SelectedStandardElement = null;
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
        }

        public List<ElementSave> SelectedElements
        {
            get
            {
                var hashSet = new HashSet<ElementSave>();

                foreach (TreeNode node in ElementTreeViewManager.Self.SelectedNodes)
                {
                    var item = GetComponentTreeNodeRoot(node)
                        ?.Tag as ElementSave;
                    if(item != null)
                    {
                        hashSet.Add(item);
                    }
                }

                return hashSet.ToList();
            }
        }

        public StateSave CustomCurrentStateSave
        {
            get;
            set;
        }

        public StateSave SelectedStateSave
        {
            get
            {
                if (CustomCurrentStateSave != null)
                {
                    return CustomCurrentStateSave;
                }
                else if (StateTreeViewManager.Self.SelectedNode != null)
                {
                    return StateTreeViewManager.Self.SelectedNode.Tag as StateSave;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                StateTreeViewManager.Self.Select(value);
            }
        }

        public StateSave SelectedStateSaveOrDefault
        {
            get
            {
                if (SelectedStateSave != null)
                {
                    return SelectedStateSave;
                }
                else if (SelectedElement != null)
                {
                    return SelectedElement.DefaultState;
                }
                else
                {
                    return null;
                }
            }
        }

        public StateSaveCategory SelectedStateCategorySave
        {
            get
            {
                var node = StateTreeViewManager.Self.SelectedNode;
                if (node != null)
                {
                    if (node.Tag is StateSaveCategory)
                    {
                        return node.Tag as StateSaveCategory;
                    }
                    else if (node.Tag is StateSave && node.Parent != null)
                    {
                        return node.Parent.Tag as StateSaveCategory;
                    }
                }

                return null;
            }
            set
            {
                StateTreeViewManager.Self.Select(value);
            }
        }

        public InstanceSave SelectedInstance
        {
            get
            {
                if (ElementTreeViewManager.Self.SelectedNode != null && ElementTreeViewManager.Self.SelectedNode.IsInstanceTreeNode())
                {
                    return ElementTreeViewManager.Self.SelectedNode.Tag as InstanceSave;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != SelectedInstance)
                {
                    if (value != null)
                    {
                        ElementSave parent = value.ParentContainer;

                        ElementTreeViewManager.Self.Select(value, parent);

                        if(parent != null && SelectedElement == null)
                        {
                            SelectedElement = parent;
                        }
                    }
                    else if (value == null && SelectedInstance != null)
                    {
                        ElementSave selected = SelectedElement;

                        ElementTreeViewManager.Self.SelectedNode = null;

                        SelectedElement = selected;
                    }
                }
            }
        }

        public IEnumerable<InstanceSave> SelectedInstances
        {
            get
            {
                // For performance reasons I was creating the instances here, but now I'm going to
                // simply return a copy of the list so that loops don't throw exceptions in foreach's if the list modifies
                List<InstanceSave> list = new List<InstanceSave>();

                foreach (TreeNode node in ElementTreeViewManager.Self.SelectedNodes)
                {
                    if (node.IsInstanceTreeNode())
                    {
                        list.Add(node.Tag as InstanceSave);
                    }
                }

                return list;
            }
            set
            {
                List<InstanceSave> list = new List<InstanceSave>();

                foreach (var item in value)
                {
                    list.Add(item);
                }

                ElementTreeViewManager.Self.Select(list);
            }

        }

        public VariableSave SelectedVariableSave
        {
            set
            {
                mSelectedVariableSave = value;
            }
            get
            {
                return mSelectedVariableSave;
            }

        }

        /// <summary>
        /// Returns the name of the selected entry in the property grid.
        /// There may not be a VariableSave backing the selection as the 
        /// value may be null in the StateSave
        /// </summary>
        public string SelectedVariableName
        {
            get
            {
                if (SelectedVariableSave != null)
                {
                    return SelectedVariableSave.GetRootName();
                }
                else
                {
                    return null;
                }
            }
        }

        public TreeNode SelectedTreeNode
        {
            get
            {
                return ElementTreeViewManager.Self.SelectedNode;
            }
        }

        public IEnumerable<TreeNode> SelectedTreeNodes
        {
            get
            {
                return ElementTreeViewManager.Self.SelectedNodes;
            }
        }

        public RecursiveVariableFinder SelectedRecursiveVariableFinder
        {
            get
            {
                if (SelectedInstance != null)
                {
                    return new RecursiveVariableFinder(SelectedInstance, SelectedElement);
                }
                else
                {
                    return new RecursiveVariableFinder(SelectedStateSave);
                }
            }
        }

        public IPositionedSizedObject SelectedIpso
        {
            get
            {
                return SelectionManager.Self.SelectedGue;
            }
            set
            {
                SelectionManager.Self.SelectedGue = value as GraphicalUiElement;
            }
        }

        public List<GraphicalUiElement> SelectedIpsos
        {
            get
            {
                return SelectionManager.Self.SelectedGues;
            }

        }

        public StateStackingMode StateStackingMode
        {
            get
            {
                if(stateView == null)
                {
                    throw new Exception("Need to call Initialize first");
                }
                return stateView.StateStackingMode;
            }
            set
            {
                stateView.StateStackingMode = value;
            }
        }

        public VariableSave SelectedBehaviorVariable
        {
            get
            {
                return PropertyGridManager.Self.SelectedBehaviorVariable;
            }

            set
            {
                PropertyGridManager.Self.SelectedBehaviorVariable = value;
            }
        }

        #endregion

        #region Methods

        private SelectedState()
        {

        }

        public void UpdateToSelectedElement()
        {
            GumCommands.Self.GuiCommands.RefreshStateTreeView();

            var stateBefore = SelectedStateSave;

            if (SelectedElement != null && 
                (SelectedStateSave == null || SelectedElement.AllStates.Contains(SelectedStateSave) == false) &&
                SelectedElement.States.Count > 0
                )
            {
                
                SelectedStateSave = SelectedElement.States[0];
            }
            else if (SelectedElement == null)
            {
                SelectedStateSave = null;

            }

            if(stateBefore == SelectedStateSave)
            {
                // If the state changed (element changed) then no need to force the UI again
                PropertyGridManager.Self.RefreshUI();
            }

            WireframeObjectManager.Self.RefreshAll(false);

            SelectionManager.Self.Refresh();
            
            MenuStripManager.Self.RefreshUI();

            PluginManager.Self.ElementSelected(SelectedElement);

        }

        public void UpdateToSelectedBehavior()
        {
            GumCommands.Self.GuiCommands.RefreshStateTreeView();

            PropertyGridManager.Self.RefreshUI();

            WireframeObjectManager.Self.RefreshAll(false);

            MenuStripManager.Self.RefreshUI();

            // Although plugins could just listen for behavior changes, and
            // assume that means no elements are selected, that's a bit of a pain.
            // A behavior may just care about whether an element is selected or not.
            PluginManager.Self.ElementSelected(null);
            PluginManager.Self.BehaviorSelected(SelectedBehavior);

        }

        public void UpdateToSelectedBehaviorVariable()
        {
            MenuStripManager.Self.RefreshUI();
        }

        public void UpdateToSelectedStateSave()
        {
            if(StateStackingMode == StateStackingMode.SingleState)
            {
                // reset everything. This is slow, but is easy
                WireframeObjectManager.Self.RefreshAll(true);
            }
            else
            {

                var currentGue = WireframeObjectManager.Self.GetSelectedRepresentation();

                if(currentGue == null)
                {
                    currentGue = WireframeObjectManager.Self.RootGue;
                }

                if(currentGue != null && this.SelectedStateSave != null)
                {
                    // Applying a state just stacks it on top of the current
                    currentGue.ApplyState(this.SelectedStateSave);
                }
            }

            SelectionManager.Self.Refresh();



            if (SelectedStateSave != null)
            {
                StateTreeViewManager.Self.Select(SelectedStateSave);
            }
            else if (SelectedStateCategorySave != null)
            {
                StateTreeViewManager.Self.Select(SelectedStateCategorySave);
            }

            PropertyGridManager.Self.RefreshUI();

            MenuStripManager.Self.RefreshUI();
        }

        public void UpdateToSelectedInstanceSave()
        {
            if (SelectedInstance != null)
            {
                ElementSave parent = SelectedInstance.ParentContainer;

                ProjectVerifier.Self.AssertIsPartOfProject(parent);

                SelectedElement = SelectedInstance.ParentContainer;
            }

            GumCommands.Self.GuiCommands.RefreshStateTreeView();

            if (SelectedElement != null && (SelectedStateSave == null || SelectedElement.AllStates.Contains(SelectedStateSave) == false))
            {
                SelectedStateSave = SelectedElement.States[0];
            }

            if (WireframeObjectManager.Self.ElementShowing != this.SelectedElement)
            {
                WireframeObjectManager.Self.RefreshAll(false);
            }

            SelectionManager.Self.Refresh();

            MenuStripManager.Self.RefreshUI();

            //PropertyGridManager.Self.RefreshUI();
            GumCommands.Self.GuiCommands.RefreshPropertyGrid();

            PluginManager.Self.InstanceSelected(SelectedElement, SelectedInstance);
        }

        public List<ElementWithState> GetTopLevelElementStack()
        {
            List<ElementWithState> toReturn = new List<ElementWithState>();

            if (SelectedElement != null)
            {
                ElementWithState item = new ElementWithState(SelectedElement);
                if (this.SelectedStateSave != null)
                {
                    item.StateName = this.SelectedStateSave.Name;
                }
                toReturn.Add(item);


            }

            return toReturn;
        }

        #endregion

    }

    public static class IEnumerableExtensionMethods
    {
        public static int GetCount(this IEnumerable<InstanceSave> enumerable)
        {
            int toReturn = 0;


            foreach (var item in enumerable)
            {
                toReturn++;
            }

            return toReturn;
        }
    }


}
