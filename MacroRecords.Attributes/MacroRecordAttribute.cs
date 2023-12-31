﻿using System;

namespace RhoMicro.MacroRecords
{
    /// <summary>
    /// Informs the macro record generator to generate a record from
    /// the annotated partial struct or class declaration.
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, Inherited = false)]
    public sealed class MacroRecordAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the visibility of the generated constructor.
        /// </summary>
        public Visibility ConstructorVisibility { get; set; } = Visibility.Private;
        /// <summary>
        /// Gets or sets additional options for the generated record.
        /// </summary>
        public RecordOptions Options { get; set; } = RecordOptions.Default;

        /// <summary>
        /// Gets a value indicating whether the <see cref="RecordOptions.DebuggerDisplay"/> flag is set on <see cref="Options"/>.
        /// </summary>
        public Boolean GenerateDebuggerDisplay => Options.HasFlag(RecordOptions.DebuggerDisplay);
        /// <summary>
        /// Gets a value indicating whether the <see cref="RecordOptions.Constructor"/> flag is set on <see cref="Options"/>.
        /// </summary>
        public Boolean GenerateConstructor => Options.HasFlag(RecordOptions.Constructor);
        /// <summary>
        /// Gets a value indicating whether the <see cref="RecordOptions.EmptyMember"/> flag is set on <see cref="Options"/>.
        /// </summary>
        public Boolean GenerateEmptyMember => Options.HasFlag(RecordOptions.EmptyMember);
        /// <summary>
        /// Gets a value indicating whether the <see cref="RecordOptions.ExplicitConversion"/> flag is set on <see cref="Options"/>.
        /// </summary>
        public Boolean GenerateExplicitConversion => Options.HasFlag(RecordOptions.ExplicitConversion);
        /// <summary>
        /// Gets a value indicating whether the <see cref="RecordOptions.ImplicitConversion"/> flag is set on <see cref="Options"/>.
        /// </summary>
        public Boolean GenerateImplicitConversion => Options.HasFlag(RecordOptions.ImplicitConversion);
        /// <summary>
        /// Gets a value indicating whether the <see cref="RecordOptions.CustomEquality"/> flag is set on <see cref="Options"/>.
        /// </summary>
        public Boolean HasCustomEquality => Options.HasFlag(RecordOptions.CustomEquality);
        /// <summary>
        /// Gets a value indicating whether the <see cref="RecordOptions.Validated"/> flag is set on <see cref="Options"/>.
        /// </summary>
        public Boolean HasValidation => Options.HasFlag(RecordOptions.Validated);
        /// <summary>
        /// Gets a value indicating whether the <see cref="RecordOptions.ToString"/> flag is set on <see cref="Options"/>.
        /// </summary>
        public Boolean HasToString => Options.HasFlag(RecordOptions.ToString);
    }
}
