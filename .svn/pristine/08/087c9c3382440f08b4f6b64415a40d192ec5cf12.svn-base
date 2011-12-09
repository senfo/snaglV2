//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System;
using Berico.Common.Events;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace Berico.SnagL.Infrastructure.Data.Attributes
{
    /// <summary>
    /// Represents an attribute that is associated with a node or edge.  This class
    /// represents the attribute itself and not the value.  The value of an attribute
    /// is represented by the AttributeValue class.
    /// </summary>
    /// <exception cref="System.ArgumentNullException">Thrown in the event that a provided property value is null</exception>
    [DataContract(Name = "Attribute")]
    public class Attribute
    {
        // Using this constant allows us to not need access to the actual Interface
        private const string SIMILARITY_MEASURE_INTERFACE_NAME = "ISimilarityMesure";

        private string name;
        private bool visible;
        private string preferredSimilarityMeasure = string.Empty;
        private SemanticType semanticType = SemanticType.Unknown;

        /// <summary>
        /// Parameterless constructor used for deserializing
        /// </summary>
        public Attribute() { }

        /// <summary>
        /// Initializes a new instance of the Berico.LinkAnalysis.Model.Attribute 
        /// class with the provided name.  This Attribute instance will default to
        /// being visible.
        /// </summary>
        /// <param name="_name">The name of the attribute</param>
        public Attribute(string _name) : this(_name, null, SemanticType.Unknown, true) { }

        /// <summary>
        /// Initializes a new instance of the Berico.LinkAnalysis.Model.Attribute 
        /// class with the provided name and visibilty setting.
        /// </summary>
        /// <param name="_name">The name of the attribute</param>
        /// <param name="_visible">Whether or not the attribute should
        /// be visible (in the UI) to the user</param>
        public Attribute(string _name, bool _visible) : this(_name, null, SemanticType.Unknown, _visible) { }

        /// <summary>
        /// Initializes a new instance of the Berico.LinkAnalysis.Model.Attribute 
        /// class with the provided name and preferred similarity measure.  This
        /// Attribute instance will default to being visible.
        /// </summary>
        /// <param name="_name">The name of the attribute</param>
        /// <param name="_preferredSimilarityMeasure">The preferred similarity measure that should be
        /// used by this attribute</param>
        public Attribute(string _name, Type _preferredSimilarityMeasure) : this(_name, _preferredSimilarityMeasure, SemanticType.Unknown, true) { }

        /// <summary>
        /// Initializes a new instance of the Berico.LinkAnalysis.Model.Attribute 
        /// class with the provided name and semantic type.  ThisAttribute
        /// instance will default to being visible.
        /// </summary>
        /// <param name="_name">The name of the attribute</param>
        /// <param name="_semanticType">The semantic type for this attribute</param>
        public Attribute(string _name, SemanticType _semanticType) : this(_name, null, _semanticType, true) { }

        /// <summary>
        /// Initializes a new instance of the Berico.LinkAnalysis.Model.Attribute 
        /// class with the provided name, preferred similarity measure and the
        /// semantic type.  This Attribute instance will default to being visible.
        /// </summary>
        /// <param name="_name">The name of the attribute</param>
        /// <param name="_preferredSimilarityMeasure">The preferred similarity measure that should be
        /// used by this attribute</param>
        /// <param name="_semanticType">The semantic type for this attribute</param>
        public Attribute(string _name, Type _preferredSimilarityMeasure, SemanticType _semanticType) : this(_name, _preferredSimilarityMeasure, _semanticType, true) { }

        /// <summary>
        /// Initializes a new instance of the Berico.LinkAnalysis.Model.Attribute 
        /// class with the provided name, preferred similarity measure, the
        /// semantic type and the visibilty setting.
        /// </summary>
        /// <param name="_name">The name of the attribute</param>
        /// <param name="_preferredSimilarityMeasure">The preferred similarity measure that should be
        /// used by this attribute</param>
        /// <param name="_semanticType">The semantic type for this attribute</param>
        /// <param name="_visible">Whether or not the attribute should
        /// be visible (in the UI) to the user</param>
        /// <exception cref="System.ArgumentNullException">Thrown in the event that Name or Value are null</exception>
        public Attribute(string _name, Type _preferredSimilarityMeasure, SemanticType _semanticType, bool _visible)
        {
            // Validate parameters
            if (String.IsNullOrEmpty(_name))
                throw new ArgumentNullException("Name", "A name must be provided for this attribute");

            SetPreferredSimilarityMeasure(_preferredSimilarityMeasure);

            // Set the internal fields for the class
            this.name = _name;
            this.semanticType = _semanticType;
            this.visible = _visible;

        }

        /// <summary>
        /// Gets the actual name of this attribute
        /// </summary>
        [DataMember(IsRequired = true, Name = "Name")]
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// Gets or sets whether this property is visible to the user
        /// </summary>
        /// <exception cref="System.ComponentModel.PropertyChanged">Thrown when the property is changed</exception> 
        [DataMember(IsRequired = false, Name = "Visible")]
        public bool Visible
        {
            get { return visible; }
            set
            {
                if (value != this.visible)
                {
                    object oldValue = this.visible;
                    this.visible = value;

                    NotifyPropertyChanged("Visible", oldValue, value);
                }
            }
        }

        /// <summary>
        /// Gets the preferred similarity measure set for this attribute
        /// </summary>
        [DataMember(IsRequired = false, Name = "PreferredSimilarityMeasure")]
        public string PreferredSimilarityMeasure
        {
            get { return this.preferredSimilarityMeasure; }
            set { this.preferredSimilarityMeasure = value; }
        }

        /// <summary>
        /// Sets the preffered similarity measure, for this attribute,
        /// to the provided type
        /// </summary>
        /// <param name="type">The type that should be used as the default
        /// similarity measure for this attribute</param>
        public void SetPreferredSimilarityMeasure(Type type)
        {
            object oldValue = this.preferredSimilarityMeasure;

            // Validate the provided type
            if (type != null)
            {
                if ((type.GetInterface("ISimilarityMeasure", true)) == null)
                    throw new ArgumentException("The provided type was not a valid Similarity Measure");

                // Save the preferred similarity measure to the name of
                // the provided type class
                this.preferredSimilarityMeasure = type.FullName;
            }
            else
            {
                this.preferredSimilarityMeasure = string.Empty;
            }

            NotifyPropertyChanged("PreferredSimilarityMeasure", oldValue, this.preferredSimilarityMeasure);
        }

        /// <summary>
        /// Gets or sets the semantic (or underlying) type for this 
        /// attribute.  For example, the attribute may physically be 
        /// a string but represents a date semantically.
        /// </summary>
        [DataMember(IsRequired = false, Name = "SemanticType")]
        public SemanticType SemanticType
        {
            get { return this.semanticType; }
            set
            {
                object oldValue = this.semanticType;
                this.semanticType = value;

                NotifyPropertyChanged("SemanticType", oldValue, value);
            }
        }

        public event EventHandler<PropertyChangedEventArgs<object>> PropertyChanged;

        /// <summary>
        /// Fires the PropertyChanged event
        /// </summary>
        /// <param name="info">A string that contains the name of the property that has changed</param>
        /// <param name="oldValue">The old value</param>
        /// <param name="newValue">The new value</param>
        public void NotifyPropertyChanged(string info, object oldValue, object newValue)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs<object>(info, newValue, oldValue));
        }

        /// <summary>
        /// Provides an appropriate string value for this attribute
        /// </summary>
        /// <returns>a string representation of this attribute</returns>
        public override string ToString()
        {
            return this.name;
        }

        /// <summary>
        /// Returns the hash code for this attribute.  The hash is based off of the
        /// attribute's Name property.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code</returns>
        public override int GetHashCode()
        {
            // Get the hash code for the name field
            return this.name.GetHashCode();
        }

        /// <summary>
        /// Determines whether this instance of Berico.LinkAnalysis.Model.Attribute and a
        /// specified object, which must also be a Berico.LinkAnalysis.Model.Attribute
        /// object, have the same value.  The main source for comparison is the attribute's
        /// Name property.
        /// </summary>
        /// <param name="obj">The object to use for equality comparison</param>
        /// <returns>true if the provided object is equal to this one; otherwise false</returns>
        public override bool Equals(object obj)
        {

            if (obj == null)
                return false;

            if (this.GetType() != obj.GetType())
                return false;

            return Equals(obj as Attribute);

        }

        /// <summary>
        /// Determines whether this instance of Berico.LinkAnalysis.Model.Attribute and a
        /// specified object, which must also be a Berico.LinkAnalysis.Model.Attribute
        /// object, have the same value.  The main source for comparison is the attribute's
        /// Name property.
        /// </summary>
        /// <param name="obj">The object to use for equality comparison</param>
        /// <returns>true if the provided object is equal to this one; otherwise false</returns>
        public bool Equals(Attribute obj)
        {

            if (obj == null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (this.GetHashCode() != obj.GetHashCode())
                return false;

            return (Name.Equals(obj.Name));

        }

        /// <summary>
        /// Determines whether two specified Berico.LinkAnalysis.Model.Attribute objects
        /// are the same
        /// </summary>
        /// <param name="leftHandSide">A Berico.LinkAnalysis.Model.Attribute</param>
        /// <param name="rightHandSide">A Berico.LinkAnalysis.Model.Attribute</param>
        /// <returns>true if the value of leftHandSide is the same as the value of righthandside;
        /// otherwise, false</returns>
        public static bool operator ==(Attribute leftHandSide, Attribute rightHandSide)
        {
            // Check if leftHandSide is null
            if (ReferenceEquals(leftHandSide, null))
                return ReferenceEquals(rightHandSide, null);

            return (leftHandSide.Equals(rightHandSide));
        }

        /// <summary>
        /// Determines whether two specified Berico.LinkAnalysis.Model.Attribute objects
        /// are not equal
        /// </summary>
        /// </summary>
        /// <param name="leftHandSide">A Berico.LinkAnalysis.Model.Attribute</param>
        /// <param name="rightHandSide">A Berico.LinkAnalysis.Model.Attribute</param>
        /// <returns>true if the value of lefthandside is different from the value of righthandside;
        /// otherwise, false</returns>
        public static bool operator !=(Attribute leftHandSide, Attribute rightHandSide)
        {
            return !(leftHandSide == rightHandSide);
        }

    }
}