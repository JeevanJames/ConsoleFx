#region --- License & Copyright Notice ---
/*
ConsoleFx CLI Library Suite
Copyright 2015-2020 Jeevan James

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion

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
        public abstract void Handle(PrompterFlow flow);
    }

    public sealed class AddQuestionAction : FlowUpdateAction
    {
        public AddQuestionAction(Question question, AddLocation location, string name)
        {
            Question = question ?? throw new ArgumentNullException(nameof(question));
            Location = location;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public AddQuestionAction(Question question)
        {
            Question = question;
            Location = null;
            Name = null;
        }

        public Question Question { get; }

        public AddLocation? Location { get; }

        public string Name { get; }

        /// <inheritdoc />
        public override void Handle(PrompterFlow flow)
        {
            if (Location is null)
                flow.Add(Question);
            else
            {
                int index = flow.IndexOf(item => item is Question q
                    && string.Equals(q.Name, Name, StringComparison.OrdinalIgnoreCase));
                if (index < 0)
                    flow.Add(Question);
                else if (Location == AddLocation.BeforeItem)
                    flow.Insert(index, Question);
                else
                    flow.Insert(index + 1, Question);
            }
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
        public override void Handle(PrompterFlow flow)
        {
            int index = flow.IndexOf(item => item is Question q
                && string.Equals(q.Name, Question.Name, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
                flow[index] = Question;
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
        public override void Handle(PrompterFlow flow)
        {
            flow.RemoveFirst(item => item is Question q && string.Equals(q.Name, Name));
        }
    }
}
