// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleFx.Prompter
{
    public sealed class UpdateFlowItem : PromptItem
    {
        public UpdateFlowItem(UpdateFlowAction updateFlowAction)
        {
            UpdateFlowAction = updateFlowAction;
        }

        public UpdateFlowAction UpdateFlowAction { get; }
    }

    /// <summary>
    ///     Allows questions and answers to be dynamically added, removed or modified at any point
    ///     during the prompter flow.
    /// </summary>
    public sealed class AsyncUpdateFlowItem : PromptItem
    {
        public AsyncUpdateFlowItem(AsyncUpdateFlowAction updateFlowAction)
        {
            UpdateFlowAction = updateFlowAction;
        }

        public AsyncUpdateFlowAction UpdateFlowAction { get; }
    }

    public delegate Task<IEnumerable<FlowUpdateAction>> AsyncUpdateFlowAction(dynamic answers);

    public delegate IEnumerable<FlowUpdateAction> UpdateFlowAction(dynamic answers);

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
