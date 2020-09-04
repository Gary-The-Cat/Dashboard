using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Commands;
using Shared.Commands.BasicCommands;
using Shared.Commands.Interfaces;
using System;
using System.Collections.Generic;

namespace Dashboard.Tests
{
    [TestClass]
    public class CommandManagerTests
    {
        [TestMethod]
        private void BasicPrimitiveTest()
        {
            var initialValue = 100;

            void Subtract(int value)
            {
                initialValue -= value;
            }

            void Add(int value)
            {
                initialValue += value;
            }

            CommandManager manager = new CommandManager();

            var testCommand = new AddValueCommand(
                Add,
                Subtract,
                10);

            manager.ExecuteCommand(testCommand);
            manager.ExecuteCommand(testCommand);

            Assert.AreEqual(initialValue, 120);
            Assert.AreEqual(manager.Undo(), true);
            Assert.AreEqual(manager.Undo(), true);

            Assert.AreEqual(initialValue, 100);
            Assert.AreEqual(manager.Undo(), null);
            Assert.AreEqual(initialValue, 100);

            Assert.AreEqual(manager.Redo(), true);
            Assert.AreEqual(manager.Redo(), true);
            Assert.AreEqual(manager.Redo(), null);
            Assert.AreEqual(initialValue, 120);
        }
        
        [TestMethod]
        private void BasicListTest()
        {
            var list = new List<string>();

            void AddItemToList(string item)
            {
                list.Add(item);
            }

            bool RemoveItemFromList(string item)
            {
                return list.Remove(item);
            }

            var manager = new CommandManager();

            manager.ExecuteCommand(new AddToListCommand<string>(AddItemToList, RemoveItemFromList, "Luke"));
            manager.ExecuteCommand(new AddToListCommand<string>(AddItemToList, RemoveItemFromList, "Taylor"));
            manager.ExecuteCommand(new AddToListCommand<string>(AddItemToList, RemoveItemFromList, "Melissa"));

            Assert.AreEqual(list.Count, 3);

            manager.Undo();

            Assert.AreEqual(list.Count, 2);

            manager.Redo();
            manager.Redo();
            manager.Redo();

            Assert.AreEqual(list.Count, 3);

            manager.Undo();
            manager.Undo();
            manager.Undo();

            manager.ExecuteCommand(new AddToListCommand<string>(AddItemToList, RemoveItemFromList, "New Test"));

            Assert.AreEqual(list.Count, 1);
            Assert.AreEqual(manager.Redo(), null);
        }
    }

    public class AddValueCommand : IUndoableCommand
    {
        private Action<int> addAction;
        private Action<int> subtractAction;
        private int value;

        public AddValueCommand(
            Action<int> addAction,
            Action<int> subtractAction,
            int value)
        {
            this.addAction = addAction;
            this.subtractAction = subtractAction;
            this.value = value;
        }

        public bool Execute()
        {
            addAction(value);

            return true;
        }

        public bool Redo()
        {
            addAction(value);

            return true;
        }

        public bool Undo()
        {
            subtractAction(value);

            return true;
        }
    }
}
