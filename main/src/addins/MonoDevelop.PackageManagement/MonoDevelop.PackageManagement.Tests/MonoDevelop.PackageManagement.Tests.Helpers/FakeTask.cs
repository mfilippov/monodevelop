﻿//
// FakeTask.cs
//
// Author:
//       Matt Ward <matt.ward@xamarin.com>
//
// Copyright (c) 2014 Xamarin Inc. (http://xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using MonoDevelop.PackageManagement;

namespace MonoDevelop.PackageManagement.Tests.Helpers
{
	class FakeTask<TResult> : ITask<TResult>
	{
		public bool IsStartCalled;
		public bool IsCancelCalled;

		public bool RunTaskSynchronously;

		Func<TResult> function;
		Action<ITask<TResult>> continueWith;

		public FakeTask (Func<TResult> function, Action<ITask<TResult>> continueWith, bool runTaskSynchronously)
		{
			this.function = function;
			this.continueWith = continueWith;
			RunTaskSynchronously = runTaskSynchronously;
			Exception = new AggregateException (new Exception ("FakeTaskAggregateInnerException"));
		}

		public void Start ()
		{
			IsStartCalled = true;
			if (RunTaskSynchronously) {
				ExecuteTaskCompletely ();
			}
		}

		public TResult Result { get; set; }

		public void ExecuteTaskCompletely ()
		{
			ExecuteTaskButNotContinueWith ();
			ExecuteContinueWith ();
		}

		public TResult ExecuteTaskButNotContinueWith ()
		{
			Result = function ();
			return Result;
		}

		public void ExecuteContinueWith ()
		{
			continueWith (this);
		}

		public void Cancel ()
		{
			IsCancelCalled = true;
		}

		public bool IsCancelled { get; set; }

		public bool IsFaulted { get; set; }

		public AggregateException Exception { get; set; }
	}
}

