﻿using DotMessenger.Contract;

namespace ExampleConsoleInMemory;

internal record Message(string Content) : IMessage;
