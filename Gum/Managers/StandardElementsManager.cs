﻿using System;
using System.Collections.Generic;
using Gum.DataTypes;
using Gum.DataTypes.Variables;
using RenderingLibrary.Graphics;
using Gum.RenderingLibrary;

#if GUM
using WpfDataUi.Controls;
using Gum.Plugins;
using Gum.PropertyGridHelpers.Converters;
#endif
using Vector2 = System.Numerics.Vector2;
using Matrix = System.Numerics.Matrix4x4;

namespace Gum.Managers
{
    #region Enums

    public enum TextureAddress
    {
        EntireTexture,
        Custom,
        DimensionsBased
    }

    public enum ChildrenLayout
    {
        Regular,
        TopToBottomStack,
        LeftToRightStack,
        AutoGridHorizontal,
        AutoGridVertical

    }

    #endregion

    public class StandardElementsManager
    {
        #region Enums

        public enum DimensionVariableAction
        {
            ExcludeFileOptions,
            AllowFileOptions,
            DefaultToPercentageOfFile
        }

        #endregion

        #region Fields

        Dictionary<string, StateSave> mDefaults;

        static StandardElementsManager mSelf;

        #endregion

        #region Properties

        public IEnumerable<string> DefaultTypes
        {
            get
            {
                foreach (var kvp in mDefaults)
                {
                    yield return kvp.Key;
                }
            }
        }

        public static StandardElementsManager Self
        {
            get
            {
                if (mSelf == null)
                {
                    mSelf = new StandardElementsManager();
                }
                return mSelf;
            }
        }

        public string DefaultType
        {
            get
            {
                return "Container";
            }
        }

        public Dictionary<string, StateSave> DefaultStates => mDefaults;

        #endregion

        public void Initialize()
        {
            RefreshDefaults();
        }

        public void RefreshDefaults()
        {
            mDefaults = new Dictionary<string, StateSave>();

            // Eventually this would get read from somewhere like an XML file
            // or a CSV file, but for
            // now we'll just use hard values.


            {
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //                                                     Text                                                           //
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                var stateSave = new StateSave();
                stateSave.Name = "Default";

                AddPositioningVariables(stateSave, includeBaseline: true);

                AddDimensionsVariables(stateSave, 100, 50, DimensionVariableAction.ExcludeFileOptions);

                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "bool", Value = true, Name = "Visible" });

                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "string", Value = "Hello", Name = "Text", Category = "Text" });

                // Okay so here's some info on this value.
                // It would be nice to be able to select whether 
                // a Text should or should not be localized in Gum.
                // The problem is that when we do the localization, we
                // only have access to the GraphicalUiElement, not the InstanceSave.
                // Sure, we could get the reference, but what about at runtime in a game?
                // We ultimately want this value to be saved in the GraphicalUiElement so it
                // can be applied at runtime too. This is a pain to do, since it would require
                // changes to SkiaGum and possibly to FRB plugins. It's a bigger project, so until
                // then, we'll leave this out and put it back in when it's time.
                //stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "bool", Value = true, Name = "Apply Localization", Category = "Text" });
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "HorizontalAlignment", Value = HorizontalAlignment.Left, Name = "HorizontalAlignment", Category = "Text" });
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "VerticalAlignment", Value = VerticalAlignment.Top, Name = "VerticalAlignment", Category = "Text" });
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "int?", Value = null, Name = "MaxLettersToShow", Category = "Text" });
                //stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "int?", Value = null, Name = "MaxNumberOfLines", Category = "Text" });
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = nameof(TextOverflowVerticalMode), Value = TextOverflowVerticalMode.SpillOver, Name = nameof(TextOverflowVerticalMode), Category = "Text" });
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = nameof(TextOverflowHorizontalMode), Value = TextOverflowHorizontalMode.TruncateWord, Name = nameof(TextOverflowHorizontalMode), Category = "Text" });

                // font:
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "bool", Value = false, Name = "UseCustomFont", Category = "Font" });

                var fontVariable = new VariableSave { SetsValue = true, Type = "string", Value = "Arial", Name = "Font", IsFont = true, Category = "Font" };
                fontVariable.PropertiesToSetOnDisplayer["IsEditable"] = true;
                stateSave.Variables.Add(fontVariable);

                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "int", Value = 18, Name = "FontSize", Category = "Font" });
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "int", Value = 0, Name = "OutlineThickness", Category = "Font" });
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "bool", Value = false, Name = "IsItalic", Category = "Font" });
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "bool", Value = false, Name = "IsBold", Category = "Font" });
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "bool", Value = true, Name = "UseFontSmoothing", Category = "Font" });
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "string", Value = "", Name = "CustomFontFile", Category = "Font", IsFile = true });
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "float", Value = 1.0f, Name = "Font Scale", Category = "Font" });
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "float", Value = 1.0f, Name = "LineHeightMultiplier", Category = "Font" });

                AddRotationVariable(stateSave);

                AddEventVariables(stateSave);

                AddStateVariable(stateSave);

                AddVariableReferenceList(stateSave);

                AddColorVariables(stateSave, includeAlpha: true);

                ApplySortValuesFromOrderInState(stateSave);

                mDefaults.Add("Text", stateSave);
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            }




            {
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //                                                     Sprite                                                         //
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                var stateSave = new StateSave();
                stateSave.Name = "Default";
                AddPositioningVariables(stateSave);
                AddDimensionsVariables(stateSave, 100, 100, DimensionVariableAction.DefaultToPercentageOfFile);
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "string", Value = "", Name = "SourceFile", IsFile = true });
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "bool", Value = true, Name = "Visible" });
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "bool", Value = false, Category = "Animation", Name = "Animate" });

                var currentChainNameVariable = new VariableSave { SetsValue = true, Type = "string", Value = null, Category = "Animation", Name = "CurrentChainName" };
                currentChainNameVariable.PropertiesToSetOnDisplayer["IsEditable"] = true;
                stateSave.Variables.Add(currentChainNameVariable);


                AddRotationVariable(stateSave);
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "bool", Value = false, Category = "Flip and Rotation", Name = "FlipHorizontal" });
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "bool", Value = false, Category = "Flip and Rotation", Name = "FlipVertical" });


                //stateSave.Variables.Add(new VariableSave { Type = "bool", Value = false, Name = "Custom Texture Coordinates", Category="Source" });
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "TextureAddress", Value = Gum.Managers.TextureAddress.EntireTexture, Name = "Texture Address", Category = "Source" });

                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "int", Value = 0, Name = "Texture Left", Category = "Source" });
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "int", Value = 0, Name = "Texture Top", Category = "Source" });
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "int", Value = 0, Name = "Texture Width", Category = "Source" });
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "int", Value = 0, Name = "Texture Height", Category = "Source" });

                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "float", Value = 0.0f, Name = "Texture Width Scale", Category = "Source" });
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "float", Value = 0.0f, Name = "Texture Height Scale", Category = "Source" });

                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "bool", Value = false, Name = "Wrap", Category = "Source" });

                AddColorVariables(stateSave);

                AddEventVariables(stateSave);

                AddStateVariable(stateSave);

                AddVariableReferenceList(stateSave);

                List<string> list = new List<string>();
                stateSave.VariableLists.Add(new VariableListSave<string> { Type = "string", Value = list, Category = "Animation", Name = "AnimationFrames" });

                ApplySortValuesFromOrderInState(stateSave);


                mDefaults.Add("Sprite", stateSave);
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            }

            {
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //                                                   Container                                                        //
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                var stateSave = new StateSave();
                stateSave.Name = "Default";


                AddPositioningVariables(stateSave);

                AddDimensionsVariables(stateSave, 150, 150, DimensionVariableAction.ExcludeFileOptions);

                stateSave.Variables.Add(new VariableSave
                {
                    SetsValue = true,
                    Category = "Children",
                    Type = "string",
                    Value = null,
                    Name = "Contained Type"
#if GUM
                    ,
                    CustomTypeConverter = new AvailableContainedTypeConverter()
#endif
                });
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Category = "Children", Type = "ChildrenLayout", Value = ChildrenLayout.Regular, Name = "Children Layout" });
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Category = "Children", Type = "float", Value = 0.0f, Name = "StackSpacing" });
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Category = "Children", Type = "bool", Value = false, Name = "Wraps Children" });

                stateSave.Variables.Add(new VariableSave { SetsValue = true, Category = "Children", Type = "int", Value = 4, Name = "AutoGridHorizontalCells" });
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Category = "Children", Type = "int", Value = 4, Name = "AutoGridVerticalCells" });


                AddClipsChildren(stateSave);

                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "bool", Value = true, Name = "Visible" });

                AddRotationVariable(stateSave);
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "bool", Value = false, Category = "Flip and Rotation", Name = "FlipHorizontal" });

                AddVariableReferenceList(stateSave);

                AddEventVariables(stateSave, defaultHasEvents: true);


                ApplySortValuesFromOrderInState(stateSave);

                mDefaults.Add("Container", stateSave);
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            }

            {
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //                                               ColoredRectangle                                                     //
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                var stateSave = new StateSave();
                stateSave.Name = "Default";

                AddPositioningVariables(stateSave);

                AddDimensionsVariables(stateSave, 50, 50, DimensionVariableAction.ExcludeFileOptions);

                AddRotationVariable(stateSave);

                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "bool", Value = true, Name = "Visible" });

                AddColorVariables(stateSave, true);

                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "Blend", Value = Blend.Normal, Name = "Blend", Category = "Rendering" });

                AddEventVariables(stateSave);

                AddStateVariable(stateSave);

                ApplySortValuesFromOrderInState(stateSave);
                
                AddVariableReferenceList(stateSave);

                mDefaults.Add("ColoredRectangle", stateSave);
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            }




            {
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //                                               Circle                                                               //
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                var stateSave = new StateSave();
                stateSave.Name = "Default";

                AddPositioningVariables(stateSave);

                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "float", Value = 16.0f, Name = "Radius" });
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "float", Value = 16.0f, Name = "Width", IsHiddenInPropertyGrid = true });
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "float", Value = 16.0f, Name = "Height", IsHiddenInPropertyGrid = true });

                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "bool", Value = true, Name = "Visible" });
                AddColorVariables(stateSave, true);

                // Although rotating a circle about its center does nothing we add rotation because you can rotate it about a different origin
                AddRotationVariable(stateSave);


                AddEventVariables(stateSave);

                AddStateVariable(stateSave);

                ApplySortValuesFromOrderInState(stateSave);

                AddVariableReferenceList(stateSave);

                mDefaults.Add("Circle", stateSave);
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            }

            {
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //                                               Rectangle                                                            //
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                var stateSave = new StateSave();
                stateSave.Name = "Default";

                AddPositioningVariables(stateSave);

                AddDimensionsVariables(stateSave, 16, 16, DimensionVariableAction.ExcludeFileOptions);

                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "bool", Value = true, Name = "Visible" });
                AddColorVariables(stateSave, true);

                AddRotationVariable(stateSave);


                AddEventVariables(stateSave);

                AddStateVariable(stateSave);

                ApplySortValuesFromOrderInState(stateSave);

                AddVariableReferenceList(stateSave);

                mDefaults.Add("Rectangle", stateSave);
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            }

            {
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //                                                     Polygon                                                        //
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                var stateSave = new StateSave();
                stateSave.Name = "Default";

                AddPositioningVariables(stateSave, addOriginVariables: false);

                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "bool", Value = true, Name = "Visible" });
                AddColorVariables(stateSave, true);

                AddRotationVariable(stateSave);

                var pointsVariable = new VariableListSave<Vector2>()
                { Name = "Points", Category = "Points" };

                pointsVariable.Value.Add(new Vector2(-32, -32));
                pointsVariable.Value.Add(new Vector2(32, -32));
                pointsVariable.Value.Add(new Vector2(32, 32));
                pointsVariable.Value.Add(new Vector2(-32, 32));
                // close it:
                pointsVariable.Value.Add(new Vector2(-32, -32));

                stateSave.VariableLists.Add(pointsVariable);

                AddStateVariable(stateSave);

                AddVariableReferenceList(stateSave);

                ApplySortValuesFromOrderInState(stateSave);

                mDefaults.Add("Polygon", stateSave);
            }


            {
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //                                                    NineSlice                                                       //
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                var stateSave = new StateSave();
                stateSave.Name = "Default";
                AddPositioningVariables(stateSave);
                AddDimensionsVariables(stateSave, 64, 64, DimensionVariableAction.ExcludeFileOptions);
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "string", Value = "", Name = "SourceFile", IsFile = true });
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "bool", Value = true, Name = "Visible" });

                AddColorVariables(stateSave);
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "Blend", Value = Blend.Normal, Name = "Blend", Category = "Rendering" });

                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "TextureAddress", Value = Gum.Managers.TextureAddress.EntireTexture, Name = "Texture Address", Category = "Source" });

                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "int", Value = 0, Name = "Texture Left", Category = "Source" });
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "int", Value = 0, Name = "Texture Top", Category = "Source" });
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "int", Value = 0, Name = "Texture Width", Category = "Source" });
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "int", Value = 0, Name = "Texture Height", Category = "Source" });

                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "float?", Value = null, Name = "CustomFrameTextureCoordinateWidth", Category = "Source" });

                AddVariableReferenceList(stateSave);

                AddEventVariables(stateSave);

                AddStateVariable(stateSave);

                AddRotationVariable(stateSave);


                ApplySortValuesFromOrderInState(stateSave);

                mDefaults.Add("NineSlice", stateSave);
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            }

            {
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //                                                     Component                                                      //
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                var stateSave = new StateSave();
                stateSave.Name = "Default";

#if GUM
                // Victor Chelaru
                // August 21, 2014
                // Not sure why we have
                // this here.  Doing so would
                // create an endless loop...
                //stateSave.Variables.Add(new VariableSave { Type = "string", Value = "Default", Name = "State", CustomTypeConverter = new AvailableStatesConverter(null)});
                // The type used to be "string" but we want to differentiate it from actual strings so we use "State"
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "State", Value = null, Name = "State", CustomTypeConverter = new AvailableStatesConverter(null) });
#endif

                ApplySortValuesFromOrderInState(stateSave);

                AddStateVariable(stateSave);

                // Not sure if component needs this - does it get values from container?
                //AddEventVariables(stateSave);

                mDefaults.Add("Component", stateSave);
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            }


            {
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //                                                    Screen                                                          //
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                var stateSave = new StateSave();
                stateSave.Name = "Default";



                ApplySortValuesFromOrderInState(stateSave);

                mDefaults.Add("Screen", stateSave);
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            }

            RefreshStateVariablesThroughPlugins();
            
            // We shouldn't do this because states above may explicitly not want to set values - like the variable for state
            //foreach (var defaultState in mDefaults.Values)
            //{
            //    foreach (var variable in defaultState.Variables)
            //    {
            //        variable.SetsValue = true;
            //    }
            //}
        }

        public static void AddClipsChildren(StateSave stateSave)
        {
            stateSave.Variables.Add(new VariableSave { SetsValue = true, Category = "Children", Type = "bool", Value = false, Name = "Clips Children" });
        }

        private void AddRotationVariable(StateSave stateSave)
        {
            var variable = new VariableSave { Type = "float", Value = 0.0f, Category = "Flip and Rotation", Name = "Rotation" };
#if GUM
            MakeDegreesAngle(variable);
#endif
            stateSave.Variables.Add(variable);
        }

        private void AddVariableReferenceList(StateSave stateSave)
        {
            var variableListSave = new VariableListSave<string>
            {
                Type = "string",
                Value = new List<string>(),
                Category = "References",
                Name = "VariableReferences"
            };
            stateSave.VariableLists.Add(variableListSave);
        }

        public void RefreshStateVariablesThroughPlugins()
        {
#if GUM
            foreach (var kvp in DefaultStates)
            {
                PluginManager.Self.ModifyDefaultStandardState(kvp.Key, kvp.Value);
            }
#endif
        }

        private void AddEventVariables(StateSave stateSave, bool defaultHasEvents = false)
        {
            stateSave.Variables.Add(
                new VariableSave
                { SetsValue = true, Type = "bool", Value = defaultHasEvents, Name = "HasEvents", Category = "Behavior", CanOnlyBeSetInDefaultState = true,
                    // We used to exclude them from instances, but there are plenty of situations where we want to hide events on an instance. It's similar to InputTransparent in XamForms
                    //ExcludeFromInstances = true
                });
            stateSave.Variables.Add(
                new VariableSave
                { SetsValue = true, Type = "bool", Value = defaultHasEvents, Name = "ExposeChildrenEvents", Category = "Behavior", CanOnlyBeSetInDefaultState = true, 
                    // We used to exclude them from instances, but there are plenty of situations where we want to hide events on an instance. It's similar to InputTransparent in XamForms
                    //ExcludeFromInstances = true 
                });
        }

        private static void AddStateVariable(StateSave stateSave)
        {
            stateSave.Variables.Add(new VariableSave
            {
                // Don't want it to set the value...
                SetsValue = false, 
                Type = "State",
                Value = "Default",
                Name = "State"
#if GUM
,
                CustomTypeConverter = new AvailableStatesConverter(null)
#endif
            });
        }

        private void ApplySortValuesFromOrderInState(StateSave stateSave)
        {
            for (int i = 0; i < stateSave.Variables.Count; i++)
            {
                stateSave.Variables[i].DesiredOrder = i;
            }
        }

        public static void AddColorVariables(StateSave stateSave, bool includeAlpha = true)
        {
            if (includeAlpha)
            {
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "int", Value = 255, Name = "Alpha", Category = "Rendering" });
            }
            stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "int", Value = 255, Name = "Red", Category = "Rendering" });
            stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "int", Value = 255, Name = "Green", Category = "Rendering" });
            stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "int", Value = 255, Name = "Blue", Category = "Rendering" });
        }

        public static void AddDimensionsVariables(StateSave stateSave, float defaultWidth, float defaultHeight, DimensionVariableAction dimensionVariableAction)
        {
            stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "float", Value = defaultWidth, Name = "Width", Category = "Dimensions" });

            var defaultValue = DimensionUnitType.Absolute;

            if(dimensionVariableAction == DimensionVariableAction.DefaultToPercentageOfFile)
            {
                defaultValue = DimensionUnitType.PercentageOfSourceFile;
            }

            VariableSave variableSave = new VariableSave { SetsValue = true, Type = typeof(DimensionUnitType).Name, Value = defaultValue, Name = "Width Units", Category = "Dimensions" };
            if (dimensionVariableAction == DimensionVariableAction.ExcludeFileOptions)
            {
                variableSave.ExcludedValuesForEnum.Add(DimensionUnitType.PercentageOfSourceFile);
                variableSave.ExcludedValuesForEnum.Add(DimensionUnitType.MaintainFileAspectRatio);
            }
            stateSave.Variables.Add(variableSave);



            stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "float", Value = defaultHeight, Name = "Height", Category = "Dimensions" });

            variableSave = new VariableSave { SetsValue = true, Type = typeof(DimensionUnitType).Name, Value = defaultValue, Name = "Height Units", Category = "Dimensions" };
            if (dimensionVariableAction == DimensionVariableAction.ExcludeFileOptions)
            {
                variableSave.ExcludedValuesForEnum.Add(DimensionUnitType.PercentageOfSourceFile);
                variableSave.ExcludedValuesForEnum.Add(DimensionUnitType.MaintainFileAspectRatio);
            }
            stateSave.Variables.Add(variableSave);
        }

        public static void AddPositioningVariables(StateSave stateSave, bool addOriginVariables = true, bool includeBaseline = false)
        {
            List<object> xUnitsExclusions = new List<object>();
            xUnitsExclusions.Add(PositionUnitType.PixelsFromTop);
            xUnitsExclusions.Add(PositionUnitType.PercentageHeight);
            xUnitsExclusions.Add(PositionUnitType.PixelsFromBottom);
            xUnitsExclusions.Add(PositionUnitType.PixelsFromCenterY);
            xUnitsExclusions.Add(PositionUnitType.PixelsFromCenterYInverted);
            xUnitsExclusions.Add(PositionUnitType.PixelsFromBaseline);

            List<object> yUnitsExclusions = new List<object>();
            yUnitsExclusions.Add(PositionUnitType.PixelsFromLeft);
            yUnitsExclusions.Add(PositionUnitType.PixelsFromCenterX);
            yUnitsExclusions.Add(PositionUnitType.PercentageWidth);
            yUnitsExclusions.Add(PositionUnitType.PixelsFromRight);


            stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "float", Value = 0.0f, Name = "X", Category = "Position" });
            stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = typeof(PositionUnitType).Name, Value = PositionUnitType.PixelsFromLeft, Name = "X Units", Category = "Position", ExcludedValuesForEnum = xUnitsExclusions });

            stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "float", Value = 0.0f, Name = "Y", Category = "Position" });
            stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = typeof(PositionUnitType).Name, Value = PositionUnitType.PixelsFromTop, Name = "Y Units", Category = "Position", ExcludedValuesForEnum = yUnitsExclusions });

            if(addOriginVariables)
            {
                stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = nameof(HorizontalAlignment), Value = HorizontalAlignment.Left, Name = "X Origin", Category = "Position" });

                var verticalAlignmentVariable =
                    new VariableSave { SetsValue = true, Type = nameof(VerticalAlignment), Value = VerticalAlignment.Top, Name = "Y Origin", Category = "Position" };
                if(includeBaseline == false)
                {
                    verticalAlignmentVariable.ExcludedValuesForEnum.Add(VerticalAlignment.TextBaseline);
                }
                stateSave.Variables.Add(verticalAlignmentVariable);
            }

            stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "string", Value = null, Name = "Guide", Category = "Position" });
#if GUM
            AddParentVariables(stateSave);
#endif
        }

#if GUM
        private static void AddParentVariables(StateSave stateSave)
        {
            VariableSave variableSave = new VariableSave();
            variableSave.SetsValue = true;
            variableSave.Type = "string";
            variableSave.Name = "Parent";
            variableSave.Category = "Parent";
            variableSave.CanOnlyBeSetInDefaultState = true;
            variableSave.CustomTypeConverter = new AvailableInstancesConverter() { IncludeScreenBounds = true };

            variableSave.PropertiesToSetOnDisplayer["IsEditable"] = true;

            stateSave.Variables.Add(variableSave);

            stateSave.Variables.Add(new VariableSave { SetsValue = true, Type = "bool", Value = false, Name = nameof(GraphicalUiElement.IgnoredByParentSize), Category = "Parent" });
        }

        public static void MakeDegreesAngle(VariableSave variableSave)
        {
            variableSave.PreferredDisplayer = typeof(AngleSelectorDisplay);
            variableSave.PropertiesToSetOnDisplayer[nameof(AngleSelectorDisplay.TypeToPushToInstance)] = AngleType.Degrees;
        }
#endif

        public StateSave GetDefaultStateFor(string type, bool throwExceptionOnMissing = true)
        {
            if (mDefaults == null)
            {
                throw new Exception("You must first call Initialize on StandardElementsManager before calling this function");
            }
            if(string.IsNullOrEmpty(type))
            {
                return null;
            }
            if(mDefaults.ContainsKey(type))
            {
                return mDefaults[type];

            }
            else
            {

                StateSave customState = null;
#if GUM
                customState = PluginManager.Self.GetDefaultStateFor(type);
#elif SKIA
                // In Skia we will assume that any type that comes through has a default state:
                customState = new StateSave();
                AddPositioningVariables(customState, addOriginVariables: true);
                mDefaults[type] = customState;
#endif

                if (customState == null && throwExceptionOnMissing)
                {
                    throw new InvalidOperationException(
                        $"Could not get the default state for type {type} in either the default or through plugins");
                }
                else
                {
                    return customState;
                }
            }
        }

        public bool IsDefaultType(string type)
        {
            return mDefaults.ContainsKey(type);
        }

        public void PopulateProjectWithDefaultStandards(GumProjectSave gumProjectSave)
        {
            if (mDefaults == null)
            {
                throw new Exception("You must first call Initialize on this StandardElementsManager");
            }


            foreach (KeyValuePair<string, StateSave> kvp in mDefaults)
            {

                string type = kvp.Key;

                if (type != "Screen")
                {
                    AddStandardElementSaveInstance(gumProjectSave, type);
                }
            }
        }

        public StandardElementSave AddStandardElementSaveInstance(GumProjectSave gumProjectSave, string type)
        {
            StandardElementSave elementSave = new StandardElementSave();
            elementSave.Initialize(mDefaults[type]);
            elementSave.Name = type;

            
            gumProjectSave.StandardElementReferences.Add( new ElementReference { Name = type, ElementType = ElementType.Standard});
            gumProjectSave.StandardElements.Add( elementSave);

            return elementSave;
        }

    }
}
