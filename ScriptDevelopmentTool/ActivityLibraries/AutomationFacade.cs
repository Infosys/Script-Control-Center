/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Infosys.ATR.WinUIAutomationRuntimeWrapper;
using Infosys.WEM.Infrastructure.Common;
using Designer = Infosys.WEM.AutomationActivity.Designers;
using ATRWrapperControls = Infosys.ATR.WinUIAutomationRuntimeWrapper.Controls;
using System.ComponentModel;

namespace Infosys.WEM.AutomationActivity.Libraries
{
    public sealed class MultipleScaleTemplateMatching : NativeActivity
    {
        [RequiredArgument]
        public InArgument<AutomationFacade> AutomationObject { get; set; }
        [RequiredArgument]
        public InArgument<bool> IsMultipleScaleTemplateMatching { get; set; }

        private AutomationFacade automationObject;
        private bool isMultipleScaleTemplateMatching;
        private const string ACTIVITY_NAME = "MultipleScaleTemplateMatching";

        protected override void Execute(NativeActivityContext context)
        {

            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ACTIVITY_NAME))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, ACTIVITY_NAME);

                    automationObject = context.GetValue(this.AutomationObject);
                    isMultipleScaleTemplateMatching = context.GetValue(this.IsMultipleScaleTemplateMatching);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "AutomationObject", Designer.ApplicationConstants.PARAMDIRECTION_IN, automationObject.ToString(), ACTIVITY_NAME);//TODO
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "IsMultipleScaleTemplateMatching", Designer.ApplicationConstants.PARAMDIRECTION_IN, isMultipleScaleTemplateMatching.ToString(), ACTIVITY_NAME);//TODO


                    automationObject.MultipleScaleTemplateMatching = isMultipleScaleTemplateMatching;
                }

                LogHandler.LogInfo(InformationMessages.ACTIVITY_EXIT_SUCCESS, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ACTIVITY_NAME);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(InformationMessages.ACTIVITY_EXIT_FAILURE, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ex.Message, ACTIVITY_NAME);
                throw ex;
            }

        }
    }

    public sealed class WaitForever : NativeActivity
    {
        [RequiredArgument]
        public InArgument<AutomationFacade> AutomationObject { get; set; }
        [RequiredArgument]
        public InArgument<bool> ShouldWaitForever { get; set; }

        private AutomationFacade automationObject;
        private bool shouldWaitForever;
        private const string ACTIVITY_NAME = "WaitForever";

        protected override void Execute(NativeActivityContext context)
        {

            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ACTIVITY_NAME))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, ACTIVITY_NAME);

                    automationObject = context.GetValue(this.AutomationObject);
                    shouldWaitForever = context.GetValue(this.ShouldWaitForever);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "AutomationObject", Designer.ApplicationConstants.PARAMDIRECTION_IN, automationObject.ToString(), ACTIVITY_NAME);//TODO
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "ShouldWaitForever", Designer.ApplicationConstants.PARAMDIRECTION_IN, shouldWaitForever.ToString(), ACTIVITY_NAME);//TODO


                    automationObject.WaitForever = shouldWaitForever;
                }

                LogHandler.LogInfo(InformationMessages.ACTIVITY_EXIT_SUCCESS, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ACTIVITY_NAME);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(InformationMessages.ACTIVITY_EXIT_FAILURE, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ex.Message, ACTIVITY_NAME);
                throw ex;
            }

        }
    }

    public sealed class UseTrueColorTemplateMatching : NativeActivity
    {
        [RequiredArgument]
        public InArgument<AutomationFacade> AutomationObject { get; set; }
        [RequiredArgument]
        public InArgument<bool> ShouldUseTrueColorTemplateMatching { get; set; }

        private AutomationFacade automationObject;
        private bool shouldUseTrueColorTemplateMatching;
        private const string ACTIVITY_NAME = "UseTrueColorTemplateMatching";

        protected override void Execute(NativeActivityContext context)
        {

            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ACTIVITY_NAME))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, ACTIVITY_NAME);

                    automationObject = context.GetValue(this.AutomationObject);
                    shouldUseTrueColorTemplateMatching = context.GetValue(this.ShouldUseTrueColorTemplateMatching);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "AutomationObject", Designer.ApplicationConstants.PARAMDIRECTION_IN, automationObject.ToString(), ACTIVITY_NAME);//TODO
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "ShouldUseTrueColorTemplateMatching", Designer.ApplicationConstants.PARAMDIRECTION_IN, shouldUseTrueColorTemplateMatching.ToString(), ACTIVITY_NAME);//TODO


                    automationObject.UseTrueColorTemplateMatching = shouldUseTrueColorTemplateMatching;
                }

                LogHandler.LogInfo(InformationMessages.ACTIVITY_EXIT_SUCCESS, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ACTIVITY_NAME);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(InformationMessages.ACTIVITY_EXIT_FAILURE, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ex.Message, ACTIVITY_NAME);
                throw ex;
            }

        }
    }

    public sealed class SelectAll : NativeActivity
    {
        [RequiredArgument]
        public InArgument<AutomationFacade> AutomationObject { get; set; }

        private AutomationFacade automationObject;
        private const string ACTIVITY_NAME = "SelectAll";

        protected override void Execute(NativeActivityContext context)
        {

            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ACTIVITY_NAME))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, ACTIVITY_NAME);

                    automationObject = context.GetValue(this.AutomationObject);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "AutomationObject", Designer.ApplicationConstants.PARAMDIRECTION_IN, automationObject.ToString(), ACTIVITY_NAME);//TODO

                    automationObject.SelectAll();
                }

                LogHandler.LogInfo(InformationMessages.ACTIVITY_EXIT_SUCCESS, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ACTIVITY_NAME);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(InformationMessages.ACTIVITY_EXIT_FAILURE, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ex.Message, ACTIVITY_NAME);
                throw ex;
            }

        }
    }

    public sealed class Copy : NativeActivity
    {
        [RequiredArgument]
        public InArgument<AutomationFacade> AutomationObject { get; set; }

        private AutomationFacade automationObject;
        private const string ACTIVITY_NAME = "Copy";

        protected override void Execute(NativeActivityContext context)
        {

            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ACTIVITY_NAME))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, ACTIVITY_NAME);

                    automationObject = context.GetValue(this.AutomationObject);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "AutomationObject", Designer.ApplicationConstants.PARAMDIRECTION_IN, automationObject.ToString(), ACTIVITY_NAME);//TODO

                    automationObject.Copy();
                }

                LogHandler.LogInfo(InformationMessages.ACTIVITY_EXIT_SUCCESS, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ACTIVITY_NAME);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(InformationMessages.ACTIVITY_EXIT_FAILURE, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ex.Message, ACTIVITY_NAME);
                throw ex;
            }

        }
    }

    public sealed class ClearClipboard : NativeActivity
    {
        [RequiredArgument]
        public InArgument<AutomationFacade> AutomationObject { get; set; }

        private AutomationFacade automationObject;
        private const string ACTIVITY_NAME = "ClearClipboard";

        protected override void Execute(NativeActivityContext context)
        {

            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ACTIVITY_NAME))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, ACTIVITY_NAME);

                    automationObject = context.GetValue(this.AutomationObject);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "AutomationObject", Designer.ApplicationConstants.PARAMDIRECTION_IN, automationObject.ToString(), ACTIVITY_NAME);//TODO

                    automationObject.ClearClipboard();
                }

                LogHandler.LogInfo(InformationMessages.ACTIVITY_EXIT_SUCCESS, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ACTIVITY_NAME);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(InformationMessages.ACTIVITY_EXIT_FAILURE, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ex.Message, ACTIVITY_NAME);
                throw ex;
            }

        }
    }

    public sealed class Read : NativeActivity
    {
        [RequiredArgument]
        public InArgument<AutomationFacade> AutomationObject { get; set; }
        public OutArgument<string> Result { get; set; }

        private AutomationFacade automationObject;
        private const string ACTIVITY_NAME = "Read";

        protected override void Execute(NativeActivityContext context)
        {

            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ACTIVITY_NAME))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, ACTIVITY_NAME);

                    automationObject = context.GetValue(this.AutomationObject);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "AutomationObject", Designer.ApplicationConstants.PARAMDIRECTION_IN, automationObject.ToString(), ACTIVITY_NAME);//TODO

                    Result.Set(context, automationObject.Read());
                }

                LogHandler.LogInfo(InformationMessages.ACTIVITY_EXIT_SUCCESS, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ACTIVITY_NAME);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(InformationMessages.ACTIVITY_EXIT_FAILURE, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ex.Message, ACTIVITY_NAME);
                throw ex;
            }

        }
    }

    public sealed class Paste : NativeActivity
    {
        [RequiredArgument]
        public InArgument<AutomationFacade> AutomationObject { get; set; }

        private AutomationFacade automationObject;
        private const string ACTIVITY_NAME = "Paste";

        protected override void Execute(NativeActivityContext context)
        {

            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ACTIVITY_NAME))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, ACTIVITY_NAME);

                    automationObject = context.GetValue(this.AutomationObject);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "AutomationObject", Designer.ApplicationConstants.PARAMDIRECTION_IN, automationObject.ToString(), ACTIVITY_NAME);//TODO

                    automationObject.Paste();
                }

                LogHandler.LogInfo(InformationMessages.ACTIVITY_EXIT_SUCCESS, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ACTIVITY_NAME);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(InformationMessages.ACTIVITY_EXIT_FAILURE, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ex.Message, ACTIVITY_NAME);
                throw ex;
            }

        }
    }

    public sealed class PasteString : NativeActivity
    {
        [RequiredArgument]
        public InArgument<AutomationFacade> AutomationObject { get; set; }
        [RequiredArgument]
        public InArgument<string> Text { get; set; }

        private AutomationFacade automationObject;
        private string text;
        private const string ACTIVITY_NAME = "PasteString";

        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ACTIVITY_NAME))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, ACTIVITY_NAME);

                    automationObject = context.GetValue(this.AutomationObject);
                    text = context.GetValue(this.Text);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "AutomationObject", Designer.ApplicationConstants.PARAMDIRECTION_IN, automationObject.ToString(), ACTIVITY_NAME);//TODO
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "TextToPaste", Designer.ApplicationConstants.PARAMDIRECTION_IN, text, ACTIVITY_NAME);

                    automationObject.Paste(text);
                }

                LogHandler.LogInfo(InformationMessages.ACTIVITY_EXIT_SUCCESS, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ACTIVITY_NAME);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(InformationMessages.ACTIVITY_EXIT_FAILURE, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ex.Message, ACTIVITY_NAME);
                throw ex;
            }

        }
    }

    public sealed class ReadToClipboard : NativeActivity
    {
        [RequiredArgument]
        public InArgument<AutomationFacade> AutomationObject { get; set; }
        [RequiredArgument]
        public InArgument<string> Text { get; set; }

        private AutomationFacade automationObject;
        private string text;
        private const string ACTIVITY_NAME = "ReadToClipboard";

        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ACTIVITY_NAME))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, ACTIVITY_NAME);

                    automationObject = context.GetValue(this.AutomationObject);
                    text = context.GetValue(this.Text);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "AutomationObject", Designer.ApplicationConstants.PARAMDIRECTION_IN, automationObject.ToString(), ACTIVITY_NAME);//TODO
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "TextToPaste", Designer.ApplicationConstants.PARAMDIRECTION_IN, text, ACTIVITY_NAME);

                    automationObject.ReadToClipboard(text);
                }

                LogHandler.LogInfo(InformationMessages.ACTIVITY_EXIT_SUCCESS, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ACTIVITY_NAME);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(InformationMessages.ACTIVITY_EXIT_FAILURE, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ex.Message, ACTIVITY_NAME);
                throw ex;
            }

        }
    }

    public sealed class Sleep : NativeActivity
    {
        [RequiredArgument]
        public InArgument<AutomationFacade> AutomationObject { get; set; }
        [RequiredArgument]
        public InArgument<int> SecondsToWait { get; set; }

        private AutomationFacade automationObject;
        private int seconds;
        private const string ACTIVITY_NAME = "Sleep";

        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ACTIVITY_NAME))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, ACTIVITY_NAME);

                    automationObject = context.GetValue(this.AutomationObject);
                    seconds = context.GetValue(this.SecondsToWait);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "AutomationObject", Designer.ApplicationConstants.PARAMDIRECTION_IN, automationObject.ToString(), ACTIVITY_NAME);//TODO
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "SecondsToWait", Designer.ApplicationConstants.PARAMDIRECTION_IN, seconds, ACTIVITY_NAME);

                    automationObject.Sleep(seconds);
                }

                LogHandler.LogInfo(InformationMessages.ACTIVITY_EXIT_SUCCESS, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ACTIVITY_NAME);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(InformationMessages.ACTIVITY_EXIT_FAILURE, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ex.Message, ACTIVITY_NAME);
                throw ex;
            }

        }
    }

    public sealed class ShowApplicationStartingWaitBox : NativeActivity
    {
        [RequiredArgument]
        public InArgument<AutomationFacade> AutomationObject { get; set; }
        [RequiredArgument]
        public InArgument<bool> ShowWaitBox { get; set; }

        private AutomationFacade autoObject;
        private bool showWaitBox;
        private const string ACTIVITY_NAME = "ShowApplicationStartingWaitBox";

        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ACTIVITY_NAME))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, ACTIVITY_NAME);

                    autoObject = context.GetValue(this.AutomationObject);
                    showWaitBox = context.GetValue(this.ShowWaitBox);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "AutomationObject", Designer.ApplicationConstants.PARAMDIRECTION_IN, autoObject.ToString(), ACTIVITY_NAME);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "ShowWaitBox", Designer.ApplicationConstants.PARAMDIRECTION_IN, showWaitBox.ToString(), ACTIVITY_NAME);

                    autoObject.ShowApplicationStartingWaitBox = showWaitBox;
                }

                LogHandler.LogInfo(InformationMessages.ACTIVITY_EXIT_SUCCESS, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ACTIVITY_NAME);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(InformationMessages.ACTIVITY_EXIT_FAILURE, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ex.Message, ACTIVITY_NAME);
                throw ex;
            }

        }
    }

    public sealed class Drag : NativeActivity
    {
        [RequiredArgument]
        public InArgument<AutomationFacade> AutomationObject { get; set; }
        [RequiredArgument]
        public InArgument<string> StartingCanonicalPath { get; set; }
        [RequiredArgument]
        public InArgument<string> DestinationCanonicalPath { get; set; }

        private AutomationFacade automationObject;
        private string startPath, endPath;
        private const string ACTIVITY_NAME = "Drag";

        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ACTIVITY_NAME))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, ACTIVITY_NAME);

                    automationObject = context.GetValue(this.AutomationObject);
                    startPath = context.GetValue(this.StartingCanonicalPath);
                    endPath = context.GetValue(this.DestinationCanonicalPath);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "AutomationObject", Designer.ApplicationConstants.PARAMDIRECTION_IN, automationObject.ToString(), ACTIVITY_NAME);//TODO
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "StartingCanonicalPath", Designer.ApplicationConstants.PARAMDIRECTION_IN, startPath, ACTIVITY_NAME);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "DestinationCanonicalPath", Designer.ApplicationConstants.PARAMDIRECTION_IN, endPath, ACTIVITY_NAME);

                    automationObject.Drag(startPath, endPath);
                }

                LogHandler.LogInfo(InformationMessages.ACTIVITY_EXIT_SUCCESS, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ACTIVITY_NAME);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(InformationMessages.ACTIVITY_EXIT_FAILURE, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ex.Message, ACTIVITY_NAME);
                throw ex;
            }

        }
    }

    public sealed class DragFromTo : NativeActivity
    {
        [RequiredArgument]
        public InArgument<AutomationFacade> AutomationObject { get; set; }
        [RequiredArgument]
        public InArgument<int> StartX { get; set; }
        [RequiredArgument]
        public InArgument<int> StartY { get; set; }
        [RequiredArgument]
        public InArgument<int> EndX { get; set; }
        [RequiredArgument]
        public InArgument<int> EndY { get; set; }

        private AutomationFacade automationObject;
        private int startx, starty, endx, endy;
        private const string ACTIVITY_NAME = "Drag from to";

        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ACTIVITY_NAME))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, ACTIVITY_NAME);

                    automationObject = context.GetValue(this.AutomationObject);
                    startx = context.GetValue(this.StartX);
                    starty = context.GetValue(this.StartY);
                    endx = context.GetValue(this.EndX);
                    endy = context.GetValue(this.EndY);

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "AutomationObject", Designer.ApplicationConstants.PARAMDIRECTION_IN, automationObject.ToString(), ACTIVITY_NAME);//TODO
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "Start X", Designer.ApplicationConstants.PARAMDIRECTION_IN, startx.ToString(), ACTIVITY_NAME);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "Start Y", Designer.ApplicationConstants.PARAMDIRECTION_IN, starty.ToString(), ACTIVITY_NAME);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "End X", Designer.ApplicationConstants.PARAMDIRECTION_IN, endx.ToString(), ACTIVITY_NAME);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "End Y", Designer.ApplicationConstants.PARAMDIRECTION_IN, endy.ToString(), ACTIVITY_NAME);

                    automationObject.Drag(startx, starty, endx, endy);
                }

                LogHandler.LogInfo(InformationMessages.ACTIVITY_EXIT_SUCCESS, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ACTIVITY_NAME);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(InformationMessages.ACTIVITY_EXIT_FAILURE, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ex.Message, ACTIVITY_NAME);
                throw ex;
            }

        }
    }

    public sealed class DragToPosition : NativeActivity
    {
        [RequiredArgument]
        public InArgument<AutomationFacade> AutomationObject { get; set; }
        [RequiredArgument]
        public InArgument<string> StartingCanonicalPath { get; set; }
        [RequiredArgument]
        public InArgument<int> X { get; set; }
        [RequiredArgument]
        public InArgument<int> Y { get; set; }
        [RequiredArgument]
        [Description("Specify 1 for Relative Destination Position, 2 for Absolute Destination Position")]
        public InArgument<int> DestinationType { get; set; }

        private AutomationFacade automationObject;
        private string startPath;
        private int xaxis, yaxis;
        Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.DragDestinationType destType;
        private const string ACTIVITY_NAME = "DragToPosition";

        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ACTIVITY_NAME))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId, ACTIVITY_NAME);

                    automationObject = context.GetValue(this.AutomationObject);
                    startPath = context.GetValue(this.StartingCanonicalPath);
                    xaxis = context.GetValue(this.X); yaxis = context.GetValue(this.Y);
                    switch (context.GetValue(this.DestinationType))
                    {
                        case 1:
                            destType = Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.DragDestinationType.RelativePosition;
                            break;
                        case 2:
                            destType = Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.DragDestinationType.AbsolutePosition;
                            break;
                        default:
                            return;
                    }

                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "AutomationObject", Designer.ApplicationConstants.PARAMDIRECTION_IN, automationObject.ToString(), ACTIVITY_NAME);//TODO
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "StartingCanonicalPath", Designer.ApplicationConstants.PARAMDIRECTION_IN, startPath, ACTIVITY_NAME);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "X coordinate", Designer.ApplicationConstants.PARAMDIRECTION_IN, xaxis, ACTIVITY_NAME);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "Y coordinate", Designer.ApplicationConstants.PARAMDIRECTION_IN, yaxis, ACTIVITY_NAME);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_PARAMETERS, LogHandler.Layer.Activity,
                               context.ActivityInstanceId, "DestinationType", Designer.ApplicationConstants.PARAMDIRECTION_IN, destType.ToString(), ACTIVITY_NAME);

                    automationObject.Drag(startPath, xaxis, yaxis, destType);
                }

                LogHandler.LogInfo(InformationMessages.ACTIVITY_EXIT_SUCCESS, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ACTIVITY_NAME);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(InformationMessages.ACTIVITY_EXIT_FAILURE, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ex.Message, ACTIVITY_NAME);
                throw ex;
            }

        }
    }   

    public sealed class ReadTextArea : NativeActivity
    {
        [RequiredArgument]
        public InArgument<AutomationFacade> AutomationObject { get; set; }
        [RequiredArgument]
        [Description("Absoulute X axis coordinate of image")]
        public InArgument<double> X { get; set; }
        [RequiredArgument]
        [Description("Absoulute Y axis coordinate of image")]
        public InArgument<double> Y { get; set; }
        [RequiredArgument]
        [Description("Height of image")]
        public InArgument<double> Height { get; set; }
        [RequiredArgument]
        [Description("Width of image")]
        public InArgument<double> Width { get; set; }
        [Description("Possible characters that could be present in the image")]
        public InArgument<string> Filter { get; set; }
        [Description("Text read from the image")]
        public OutArgument<string> Result { get; set; }

        private AutomationFacade automationObject;

        protected override void Execute(NativeActivityContext context)
        {
            try
            {
                //check if stop requested, if show then throw exception
                if (Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.Utilities.IsStopRequested())
                    throw new Infosys.ATR.WinUIAutomationRuntimeWrapper.Core.IAPExceptions.StopRequested();

                using (LogHandler.TraceOperations(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.WorkflowInstanceId,
                    context.ActivityInstanceId, ActivityEvents.OFFSET_REGION, ActivityControls.BASE_CONTROL))
                {
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_ENTER, LogHandler.Layer.Activity, context.ActivityInstanceId,
                        ActivityEvents.OFFSET_REGION, ActivityControls.BASE_CONTROL);

                    //in param                   
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "Offset X", Designer.ApplicationConstants.PARAMDIRECTION_IN, X);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "Offest Y ", Designer.ApplicationConstants.PARAMDIRECTION_IN, X);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "height", Designer.ApplicationConstants.PARAMDIRECTION_IN, Height);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "width", Designer.ApplicationConstants.PARAMDIRECTION_IN, Width);
                    LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_PARAMETERS, LogHandler.Layer.Activity,
                                context.ActivityInstanceId, "filter", Designer.ApplicationConstants.PARAMDIRECTION_IN, Filter);

                    //logic
                    automationObject = context.GetValue(this.AutomationObject);
                    string text = automationObject.ReadTextArea(context.GetValue(X), context.GetValue(Y), context.GetValue(Height), context.GetValue(Width), context.GetValue(Filter));

                    Result.Set(context, text);
                }

                LogHandler.LogInfo(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_EXIT_SUCCESS, LogHandler.Layer.Activity,
                        context.ActivityInstanceId);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(InformationMessages.ACTIVITY_AUTOMATIONRUNTIME_EXIT_FAILURE, LogHandler.Layer.Activity,
                        context.ActivityInstanceId, ex.Message);
                throw ex;
            }

        }

        public Activity Create(System.Windows.DependencyObject target)
        {
            return new ReadTextArea
            {
                DisplayName = "ReadTextArea",
            };
        }
    }
}
