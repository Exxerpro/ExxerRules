// <copyright file="ConcurrentSubjectDictionary.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace Sharp7.Rx.Basics;

using System.Collections.Concurrent;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

/// <summary>
/// A concurrent dictionary that manages <see cref="Subject{T}"/> instances with reference counting.
/// </summary>
/// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
/// <typeparam name="TValue">The type of the values (observable elements) in the subjects.</typeparam>
internal class ConcurrentSubjectDictionary<TKey, TValue> : IDisposable
{
    private readonly object dictionaryLock = new object();
    private readonly Func<TKey, TValue> valueFactory;
    private ConcurrentDictionary<TKey, SubjectWithRefCounter> dictionary;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConcurrentSubjectDictionary{TKey, TValue}"/> class.
    /// </summary>
    public ConcurrentSubjectDictionary()
    {
        this.dictionary = new ConcurrentDictionary<TKey, SubjectWithRefCounter>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConcurrentSubjectDictionary{TKey, TValue}"/> class with a specified equality comparer.
    /// </summary>
    /// <param name="comparer">The equality comparer to use for the keys.</param>
    public ConcurrentSubjectDictionary(IEqualityComparer<TKey> comparer)
    {
        this.dictionary = new ConcurrentDictionary<TKey, SubjectWithRefCounter>(comparer);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConcurrentSubjectDictionary{TKey, TValue}"/> class with an initial value and a specified equality comparer.
    /// </summary>
    /// <param name="initialValue">The initial value for new subjects.</param>
    /// <param name="comparer">The equality comparer to use for the keys.</param>
    public ConcurrentSubjectDictionary(TValue initialValue, IEqualityComparer<TKey> comparer)
    {
        this.valueFactory = _ => initialValue;
        this.dictionary = new ConcurrentDictionary<TKey, SubjectWithRefCounter>(comparer);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConcurrentSubjectDictionary{TKey, TValue}"/> class with an initial value.
    /// </summary>
    /// <param name="initialValue">The initial value for new subjects.</param>
    public ConcurrentSubjectDictionary(TValue initialValue)
    {
        this.valueFactory = _ => initialValue;
        this.dictionary = new ConcurrentDictionary<TKey, SubjectWithRefCounter>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConcurrentSubjectDictionary{TKey, TValue}"/> class with a value factory.
    /// </summary>
    /// <param name="valueFactory">The function to create initial values for new subjects.</param>
    public ConcurrentSubjectDictionary(Func<TKey, TValue> valueFactory = null)
    {
        this.valueFactory = valueFactory;
        this.dictionary = new ConcurrentDictionary<TKey, SubjectWithRefCounter>();
    }

    /// <summary>
    /// Gets a collection of existing keys in the dictionary.
    /// </summary>
    public IEnumerable<TKey> ExistingKeys => this.dictionary.Keys;

    /// <summary>
    /// Gets a value indicating whether the dictionary has been disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Disposes the dictionary and its managed resources.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Gets or creates an observable for the specified key.
    /// </summary>
    /// <param name="key">The key for which to get or create the observable.</param>
    /// <returns>A <see cref="DisposableItem{TValue}"/> containing the observable and a dispose action.</returns>
    public DisposableItem<TValue> GetOrCreateObservable(TKey key)
    {
        lock (this.dictionaryLock)
        {
            var subject = this.dictionary.AddOrUpdate(
                key,
                k => new SubjectWithRefCounter(this.CreateSubject(k)),
                (_, subjectWithRefCounter) =>
                {
                    subjectWithRefCounter.IncreaseCount();
                    return subjectWithRefCounter;
                });

            return new DisposableItem<TValue>(subject.Subject.AsObservable(), () => this.RemoveIfNoLongerInUse(key));
        }
    }

    /// <summary>
    /// Tries to get an observer for the specified key.
    /// </summary>
    /// <param name="key">The key for which to get the observer.</param>
    /// <param name="subject">When this method returns, contains the observer associated with the specified key, if the key is found; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the key was found and the observer was retrieved successfully; otherwise, <see langword="false"/>.</returns>
    public bool TryGetObserver(TKey key, out IObserver<TValue> subject)
    {
        if (this.dictionary.TryGetValue(key, out var subjectWithRefCount))
        {
            subject = subjectWithRefCount.Subject.AsObserver();
            return true;
        }

        subject = null;
        return false;
    }

    /// <summary>
    /// Disposes the managed resources used by the <see cref="ConcurrentSubjectDictionary{TKey, TValue}"/>.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (this.IsDisposed)
        {
            return;
        }

        if (disposing && this.dictionary != null)
        {
            foreach (var subjectWithRefCounter in this.dictionary)
            {
                subjectWithRefCounter.Value.Subject.OnCompleted();
            }

            this.dictionary.Clear();
            this.dictionary = null;
        }

        this.IsDisposed = true;
    }

    private ISubject<TValue> CreateSubject(TKey key)
    {
        if (this.valueFactory == null)
        {
            return new Subject<TValue>();
        }

        return new BehaviorSubject<TValue>(this.valueFactory(key));
    }

    private void RemoveIfNoLongerInUse(TKey variableName)
    {
        lock (this.dictionaryLock)
        {
            if (this.dictionary.TryGetValue(variableName, out var subjectWithRefCount))
            {
                if (subjectWithRefCount.DecreaseCount() < 1)
                {
                    this.dictionary.TryRemove(variableName, out _);
                }
            }
        }
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="ConcurrentSubjectDictionary{TKey, TValue}"/> class.
    /// </summary>
    ~ConcurrentSubjectDictionary()
    {
        this.Dispose(false);
    }

    /// <summary>
    /// Represents a subject with a reference counter.
    /// </summary>
    private class SubjectWithRefCounter
    {
        private int counter = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubjectWithRefCounter"/> class.
        /// </summary>
        /// <param name="subject">The subject instance.</param>
        public SubjectWithRefCounter(ISubject<TValue> subject)
        {
            this.Subject = subject;
        }

        /// <summary>
        /// Gets the subject instance.
        /// </summary>
        public ISubject<TValue> Subject { get; }

        /// <summary>
        /// Decreases the reference count.
        /// </summary>
        /// <returns>The new reference count.</returns>
        public int DecreaseCount() => Interlocked.Decrement(ref this.counter);

        /// <summary>
        /// Increases the reference count.
        /// </summary>
        /// <returns>The new reference count.</returns>
        public int IncreaseCount() => Interlocked.Increment(ref this.counter);
    }
}
