/*
 * Copyright 2004 - $Date: 2008-11-15 23:58:07 +0100 (za, 15 nov 2008) $ by PeopleWare n.v..
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#region Using

using System.ComponentModel;
using System.Diagnostics.Contracts;

using PPWCode.Vernacular.Exceptions.I;

#endregion

namespace PPWCode.Vernacular.Semantics.I
{
    /// <summary>
    /// A type that formalizes how to work with objects that represent
    /// real-world objects during only a part of there life cycle.
    /// </summary>
    /// <remarks>
    /// <a href="http://en.wikipedia.org/wiki/Jean-Jacques_Rousseau">
    /// <img src="../img/180px-Jean-Jacques_Rousseau_(painted_portrait).jpg"
    ///      align="right" /></a>
    /// <para>For several reasons, some of which are technical, some of
    /// which are best practices, some of which are more design related,
    /// classes often have only a default constructor. On the other hand
    /// often some properties cannot be given a semantically acceptable value
    /// at instantiation. This triggers a pattern where instances of the
    /// semantic class can exist in a state where it does not represent a
    /// real-world object of the type the class is intended for, i.e.,
    /// the instances do not conform to type invariants that would apply
    /// direct representations of the real-world objects. Such objects
    /// are created in a <em>wild</em> state, then
    /// go through a setup phase where a number of properties are set,
    /// and then enter a lifecycle phase where they do represent a
    /// real-world object of the type the class is intended for (they become
    /// <em>civilized</em>).
    /// Typically, by changing one or more properties, such objects
    /// can also leave the civilized state, which typically happens before
    /// the object is terminated.</para>
    /// <para>This type offers a number of methods to support this pattern.</para>
    /// <para>Normally, invariants are specified and enforced as much as possible.
    /// This is possible for all properties for which there exists a
    /// <em>civilized</em> default value that can be set in the default
    /// constructor. Typically, this is at least not possible with
    /// associations, if the association is mandatory.</para>
    /// <para>The extra rules that should apply in a <em>civilized</em> state
    /// can be checked by calling <see cref="WildExceptions"/>.
    /// <see cref="IsCivilized"/> gives a simple boolean answer about the state
    /// of the <c>IRousseauObject</c>.</para>
    /// <para><see cref="AbstractRousseauObject"/> offers hard implementations that
    /// enforce the above rules. As this interface doesn't really enforce anything through
    /// the compiler, it can be seen as a tagging interface.</para>
    /// </remarks>
    [ContractClass(typeof(IRousseauObjectContracts))]
    public interface IRousseauObject :
        ISemanticObject
    {
        /// <summary>
        /// A call to <see cref="WildExceptions"/>
        /// returns an <see cref="CompoundSemanticException.IsEmpty"/>
        /// exception.
        /// </summary>
        [Pure]
        bool IsCivilized();

        /// <summary>
        /// Build a set of <see cref="CompoundSemanticException"/> instances
        /// that tell what is wrong with this instance, with respect to
        /// <em>being civilized</em>.
        /// </summary>
        /// <returns>
        /// <para>The result comes in the form of an <strong>unclosed</strong>
        /// <see cref="CompoundSemanticException"/>, of
        /// which the set of element exceptions might be empty.</para>
        /// <para>This method should work in any state of the object.</para>
        /// <para>This method is public instead of
        /// protected to make it more easy to describe to users what the business
        /// rules for this type are.</para>
        /// </returns>
        [Pure]
        CompoundSemanticException WildExceptions();

        /// <summary>
        /// Call <see cref="WildExceptions"/>, and if the result
        /// is not <see cref="CompoundSemanticException.IsEmpty"/>,
        /// close the exception and throw it.
        /// </summary>
        /// <remarks>
        /// <para>This method has no effects. If it ends nominally,
        /// and if it throws an exception, no state is changed.</para>
        /// <para>It is not <c>[Pure]</c> however, since it changes
        /// the state of the exception to
        /// <see cref="CompoundSemanticException.Closed"/>.</para>
        /// </remarks>
        void ThrowIfNotCivilized();
    }

    // ReSharper disable InconsistentNaming
    /// <exclude />
    [ContractClassFor(typeof(IRousseauObject))]
    public abstract class IRousseauObjectContracts :
        IRousseauObject
    {
        #region IRousseauObject Members

        [Pure]
        public bool IsCivilized()
        {
            Contract.Ensures(Contract.Result<bool>() == WildExceptions().IsEmpty);

            return default(bool);
        }

        [Pure]
        public CompoundSemanticException WildExceptions()
        {
            Contract.Ensures(Contract.Result<CompoundSemanticException>() != null);
            Contract.Ensures(!Contract.Result<CompoundSemanticException>().Closed);

            return default(CompoundSemanticException);
        }

        public void ThrowIfNotCivilized()
        {
            Contract.Ensures(
                IsCivilized(),
                "Method must end nominally if this is civilized, " +
                "and is not allowed to end nominally if this is not.");
            Contract.EnsuresOnThrow<CompoundSemanticException>(
                ! IsCivilized(),
                "If this method throws a CompoundSemanticException, this" +
                "was and is not civilized.");
        }

        public abstract bool IsSerialized { get; }

        #endregion

        #region INotifyPropertyChanged Members

#pragma warning disable

        public event PropertyChangedEventHandler PropertyChanged;

#pragma warning restore

        #endregion
    }
}