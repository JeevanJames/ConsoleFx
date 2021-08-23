// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleFx.Prompter
{
    /// <summary>
    ///     Allows questions and answers to be dynamically added, removed or modified at any point
    ///     during the prompter flow.
    /// </summary>
    public sealed class UpdateFlowItem : PromptItem
    {
        public UpdateFlowItem(Func<dynamic, IEnumerable<FlowUpdateAction>> updateFlowAction)
        {
            UpdateFlowAction = updateFlowAction ?? throw new ArgumentNullException(nameof(updateFlowAction));
        }

        public UpdateFlowItem(Func<dynamic, Task<IEnumerable<FlowUpdateAction>>> asyncUpdateFlowAction)
        {
            AsyncUpdateFlowAction = asyncUpdateFlowAction
                ?? throw new ArgumentNullException(nameof(asyncUpdateFlowAction));
        }

        public Func<dynamic, IEnumerable<FlowUpdateAction>> UpdateFlowAction { get; }

        public Func<dynamic, Task<IEnumerable<FlowUpdateAction>>> AsyncUpdateFlowAction { get; }
    }

    public abstract class FlowUpdateAction
    {
        public abstract void Handle(PrompterFlow flow, int index);
    }

    public sealed class AddQuestionAction : FlowUpdateAction
    {
        public AddQuestionAction(Question question)
        {
            Question = question;
        }

        public Question Question { get; }

        /// <inheritdoc />
        public override void Handle(PrompterFlow flow, int index)
        {
            flow.Insert(index + 1, Question);
        }
    }

    public enum AddLocation
    {
        BeforeItem,
        AfterItem,
    }

    public sealed class UpdateQuestionAction : FlowUpdateAction
    {
        public UpdateQuestionAction(Question question)
        {
            Question = question ?? throw new ArgumentNullException(nameof(question));
        }

        public Question Question { get; }

        /// <inheritdoc />
        public override void Handle(PrompterFlow flow, int index)
        {
            int questionIndex = flow.IndexOf(item => item is Question q
                && string.Equals(q.Name, Question.Name, StringComparison.OrdinalIgnoreCase));
            if (questionIndex >= 0)
                flow[questionIndex] = Question;
        }
    }

    public sealed class DeleteQuestionAction : FlowUpdateAction
    {
        public DeleteQuestionAction(string name)
        {
            Name = name;
        }

        public string Name { get; }

        /// <inheritdoc />
        public override void Handle(PrompterFlow flow, int index)
        {
            flow.RemoveFirst(item => item is Question q && string.Equals(q.Name, Name));
        }
    }
}
