// Copyright (c) 2015-2021 Jeevan James
// This file is licensed to you under the Apache License, Version 2.0.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ConsoleFx.ConsoleExtensions;

namespace ConsoleFx.Prompter
{
    public sealed partial class PrompterFlow
    {
        public async Task<Answers> Ask()
        {
            var answers = new Answers(_promptItems.Count);

            EventHandler<BeforeAfterPromptEventArgs> beforePrompt = BeforePrompt;
            EventHandler<BetweenPromptEventArgs> betweenPrompts = BetweenPrompts;
            EventHandler<BeforeAfterPromptEventArgs> afterPrompt = AfterPrompt;

            for (int i = 0; i < _promptItems.Count; i++)
            {
                PromptItem promptItem = _promptItems[i];

                beforePrompt?.Invoke(this, new BeforeAfterPromptEventArgs { Prompt = promptItem });

                // If the prompt cannot be processed, continue the loop.
                if (!promptItem.CanAsk(answers))
                {
                    HandleSkipItem(promptItem, answers);
                    continue;
                }

                // Remaining events are raised only if a question is asked or static text is displayed.
                bool raiseRemainingEvents = false;

                switch (promptItem)
                {
                    case DisplayItem displayItem:
                        raiseRemainingEvents = true;
                        switch (displayItem)
                        {
                            case StaticText staticText:
                                staticText.Display(answers);
                                break;
                            case Question question:
                                HandleAsQuestion(question, answers);
                                break;
                            default:
                                throw new InvalidOperationException($"Unrecognized display item type - '{displayItem.GetType()}");
                        }

                        break;

                    case UpdateFlowItem updateItem:
                        if (updateItem.UpdateFlowAction is not null)
                            HandleAsUpdateItem(updateItem, answers, i);
                        else
                            await HandleAsAsyncUpdateItem(updateItem, answers, i).ConfigureAwait(false);
                        break;

                    default:
                        throw new InvalidOperationException($"Unrecognized prompt item type - '{promptItem.GetType()}");
                }

                if (raiseRemainingEvents)
                {
                    if (i < _promptItems.Count - 1)
                    {
                        betweenPrompts?.Invoke(this, new BetweenPromptEventArgs
                        {
                            PreviousPrompt = promptItem,
                            NextPrompt = _promptItems[i + 1],
                        });
                    }

                    afterPrompt?.Invoke(this, new BeforeAfterPromptEventArgs { Prompt = promptItem });
                }
            }

            return answers;
        }

        private static void HandleSkipItem(PromptItem promptItem, Answers answers)
        {
            // If the prompt item is a question, try assigning the default value, if available, before
            // continuing.
            if (promptItem is Question q)
            {
                object answer = q.DefaultValue.Resolve(answers);
                if (answer is not null)
                    answers.Add(q.Name, answer);
            }
        }

        private static void HandleAsQuestion(Question question, Answers answers)
        {
            if (question.Instructions.Count > 0)
            {
                foreach (FunctionOrColorString instruction in question.Instructions)
                {
                    ColorString cstr = new ColorString().Text(instruction.Resolve(answers),
                        Style.Instructions.ForeColor, Style.Instructions.BackColor);
                    ConsoleEx.PrintLine(cstr.ToString());
                }
            }

            object answer = null;
            bool validAnswer = false;
            do
            {
                object input = question.Ask(answers);

                // Validate the value returned by the Asker function, before any processing or
                // conversion.
                if (question.RawValueValidator is not null)
                {
                    ValidationResult validationResult = question.RawValueValidator(input, answers);
                    if (!validationResult.IsValid)
                    {
                        if (!string.IsNullOrWhiteSpace(validationResult.ErrorMessage))
                            ConsoleEx.PrintLine($"{Clr.Red}{validationResult.ErrorMessage}");
                        continue;
                    }
                }

                // Assign default value, if the input is null or an empty string.
                answer = input is null or string { Length: 0 } && question.DefaultValue.IsAssigned
                    ? question.DefaultValue.Resolve(answers) : input;

                // Convert the answer, if needed.
                answer = question.Convert(answer);

                // Validate the processed and converted value.
                if (question.ConvertedValueValidator is not null)
                {
                    ValidationResult validationResult = question.ConvertedValueValidator(answer, answers);
                    if (!validationResult.IsValid)
                    {
                        if (!string.IsNullOrWhiteSpace(validationResult.ErrorMessage))
                            ConsoleEx.PrintLine($"{Clr.Red}{validationResult.ErrorMessage}");
                        continue;
                    }
                }

                validAnswer = true;
            }
            while (!validAnswer);

            answers.Add(question.Name, answer);
        }

        private async Task HandleAsAsyncUpdateItem(UpdateFlowItem updateItem, Answers answers, int index)
        {
            int i = index;
            IEnumerable<FlowUpdateAction> updateActions = await updateItem.AsyncUpdateFlowAction(answers);
            foreach (FlowUpdateAction updateAction in updateActions)
                updateAction.Handle(this, i++);
        }

        private void HandleAsUpdateItem(UpdateFlowItem updateItem, Answers answers, int index)
        {
            int i = index;
            IEnumerable<FlowUpdateAction> updateActions = updateItem.UpdateFlowAction(answers);
            foreach (FlowUpdateAction updateAction in updateActions)
                updateAction.Handle(this, i++);
        }

        public event EventHandler<BeforeAfterPromptEventArgs> BeforePrompt;

        public event EventHandler<BetweenPromptEventArgs> BetweenPrompts;

        public event EventHandler<BeforeAfterPromptEventArgs> AfterPrompt;

        private static Styling _style;

        public static Styling Style
        {
            get => _style ??= Styling.NoTheme;
            set => _style = value;
        }
    }
}
