﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsQuery.Implementation
{

    /// <summary>
    /// Something that appears in the DOM (e.g., not an attribute, DomDocument, and so on)
    /// </summary>
    public abstract class DomObject: IDomObject
    {
        #region private properties

        private IDomDocument _Document;
        private int _Index;
        // This must be accessd by overriding PathID implemenetation in DomElement
        protected string _PathID;
        protected ushort _NodeNameID;

        protected abstract IDomObject CloneImplementation();

        // Do not expose this. _ParentNode should only be managed by the ParentNode property.
        private IDomContainer _ParentNode;

        #endregion
        
        #region public properties

        public abstract NodeType NodeType { get; }
        public abstract bool HasChildren { get; }
        public abstract bool InnerHtmlAllowed { get; }
        public ushort NodeNameID { get { return _NodeNameID; } }

        public virtual bool InnerTextAllowed
        {
            get { return InnerHtmlAllowed; }
        }

        public virtual bool Complete { get { return true; } }
        public virtual bool IsIndexed { get { return false; } }

        /// <summary>
        /// The full path to this node. This is calculated by requesting the parent path and adding its own ID.
        /// </summary>
        public virtual string Path
        {
            get
            {
                return ParentNode != null ? ParentNode.Path + PathID :
                    "_" + PathID;
            }
        }

        /// <summary>
        /// The DOM for this object. This is obtained by looking at its parents value until it finds a non-null
        /// Document in a parent. The value is cached locally as long as the current value of Parent remains the same.
        /// </summary>
        public virtual IDomDocument Document
        {
            get
            {
                if (_Document == null)
                {
                    _Document = ParentNode == null ? null : ParentNode.Document;
                }
                return _Document;
            }
            internal set
            {
                _Document = value;
                if (value == null && HasChildren)
                {
                    foreach (var item in ChildElements)
                    {
                        ((DomObject)item).Document = null;
                    }

                }
            }
        }

        public virtual string InnerText
        {
            get
            {
                throw new InvalidOperationException("Accessing InnerText is not valid for this element type.");
            }
            set
            {
                throw new InvalidOperationException("Assigning InnerText is not valid for this element type.");
            }
        }
        // Owner can be null (this is an unbound element)
        // if so create an arbitrary one.

        public virtual string InnerHTML
        {
            get
            {
                throw new InvalidOperationException("Accessing InnerHtml is not valid for this element type.");
            }
            set
            {
                throw new InvalidOperationException("Assigning InnerHtml is not valid for this element type.");
            }
        }

        

        public virtual INodeList ChildNodes
        {
            get
            {
                return null;
            }
        }

        public virtual IDomContainer ParentNode
        {
            get
            {
                return _ParentNode;
            }
            internal set
            {
                _ParentNode = value;
                //_ParentPath = null;
                // de-cache _Document
                Document = null;
            }
        }

        /// <summary>
        /// The element is not associated with an IDomDocument
        /// </summary>
        public bool IsDisconnected
        {
            get
            {
                return Document == null;
            }
        }


        /// <summary>
        /// Unique ID assigned when added to a dom. This is not the full path but just the ID at this level. The full
        /// path is never stored with each node to prevent having to regenerate if node trees are moved. 
        /// </summary>
        public virtual string PathID
        {
            get
            {
                // Don't actually store paths with non-element nodes as they aren't indexed and don't have children.
                // Fast read access is less important than not having to reset them when moved.
                return PathEncode(Index);
            }
            internal set
            {
                _PathID = value;
            }
        }
        
        public virtual int Depth
        {
            get
            {
                return ParentNode==null ? 0 : ParentNode.Depth + 1;
            }
        }

        public virtual IEnumerable<IDomElement> ChildElements
        {
            get
            {
                yield break;
            }
        }

        /// <summary>
        /// The element's absolute index among its siblings
        /// </summary>
        /// <returns></returns>
        public int Index
        {
            get
            {
                if (_Index < 0)
                {
                    if (ParentNode != null)
                    {
                        _Index = ParentNode.ChildNodes.IndexOf(this);
                    }
                    else
                    {
                        _Index = 0;
                    }
                }
                return _Index;
            }
            internal set
            {
                _Index = value;
                _PathID = null;
            }
        }


        public virtual string DefaultValue
        {
            get
            {
                return null;
            }

            set
            {
                throw new InvalidOperationException("DefaultValue is not valid for this node type");
            }
        }

        public virtual string NodeValue
        {
            get
            {
                return null;
            }
            set
            {
                throw new InvalidOperationException("You can't set NodeValue for this node type.");
            }
        }
        public virtual string Type
        {
            get
            {
                return null;
            }
            set
            {
                throw new InvalidOperationException("You can't set Type for this node type.");
            }
        }
        public virtual string Name
        {
            get
            {
                return null;
            }
            set
            {
                throw new InvalidOperationException("You can't set Name for this node type.");
            }
        }
        public virtual string Id
        {
            get
            {
                return null;
            }
            set
            {
                throw new InvalidOperationException("Cannot set ID for this node type.");
            }
        }

        public virtual string Value
        {
            get
            {
                return null;
            }
            set
            {
                throw new InvalidOperationException("Cannot set value for this node type.");
            }
        }

        /// <inheritdoc />
        public virtual string ClassName
        {
            get
            {
                return null;
            }
            set
            {
                throw new InvalidOperationException("ClassName is not applicable to this node type.");
            }
        }

        /// <inheritdoc />
        public IEnumerable<string> Classes
        {
            get
            {
                yield break;
            }

        }


        public virtual IAttributeCollection Attributes
        {
            get
            {
                return null;
            }
            protected set
            {
                throw new InvalidOperationException("Attributes collection is not applicable to this node type.");
            }
        }

        public virtual CSSStyleDeclaration Style
        {
            get
            {
                return null;
            }
            protected set
            {
                throw new InvalidOperationException("Style is not applicable to this node type.");
            }
        }


        public virtual string NodeName
        {
            get
            {
                return null;
            }
            set
            {
                throw new InvalidOperationException("You can't change the node name.");
            }
        }


        public virtual IDomObject FirstChild
        {
            get { return null; }
        }
        public virtual IDomObject LastChild
        {
            get { return null; }
        }
        public virtual IDomElement FirstElementChild
        {
            get { return null; }
        }
        public virtual IDomElement LastElementChild
        {
            get { return null; }
        }

        public virtual bool HasAttributes
        {
            get
            {
                return false;
            }
        }

        public virtual bool HasClasses
        {
            get
            {
                return false;
            }
        }
        public virtual bool HasStyles
        {
            get
            {
                return false;
            }
        }


        public virtual bool Selected
        {
            get { return false; }
        }

        public virtual bool Checked
        {
            get
            {
                return false;
            }
            set
            {
                throw new InvalidOperationException("Not valid for this element type.");
            }
        }

        public virtual bool ReadOnly
        {
            get
            {
                return true;
            }
            set
            {
                throw new InvalidOperationException("Not valid for this element type.");
            }
        }

        public IDomObject NextSibling
        {
            get
            {
                return ParentNode != null && ParentNode.ChildNodes.Count - 1 > Index ?
                    ParentNode.ChildNodes[Index + 1] :
                    null;
            }
        }
        public IDomObject PreviousSibling
        {
            get
            {
                return ParentNode != null && Index > 0 ?
                    ParentNode.ChildNodes[Index - 1] :
                    null;
            }
        }
        public IDomElement NextElementSibling
        {
            get
            {
                int curIndex = Index;
                var elements = ParentNode.ChildNodes;
                while (++curIndex < elements.Count)
                {
                    if (elements[curIndex].NodeType == NodeType.ELEMENT_NODE)
                    {
                        return (IDomElement)elements[curIndex];
                    }
                }
                return null;
            }
        }
        public IDomElement PreviousElementSibling
        {
            get
            {
                int curIndex = Index;
                var elements = ParentNode.ChildNodes;
                while (--curIndex >= 0)
                {
                    if (elements[curIndex].NodeType == NodeType.ELEMENT_NODE)
                    {
                        return (IDomElement)elements[curIndex];
                    }
                }
                return null;
            }
        }


        public virtual IDomObject this[int index]
        {
            get
            {
                throw new InvalidOperationException("This element type does not have children");
            }
        }

        /// <summary>
        /// Returns the value of the named attribute
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual string this[string attribute]
        {
            get
            {
                throw new InvalidOperationException("This element type does not have attributes");
            }
            set
            {
                throw new InvalidOperationException("This element type does not have attributes");
            }
        }

        #endregion

        #region public methods

        public abstract string Render();

        public virtual void Render(StringBuilder sb)
        {
            sb.Append(Render());
        }
        public virtual void Render(StringBuilder sb, DomRenderingOptions options)
        {
            sb.Append(Render());
        }
       
        /// <summary>
        /// Wraps the element in a CsQuery object
        /// </summary>
        /// <returns></returns>
        public CQ Cq()
        {
            return new CQ(this);
        }

        public virtual IDomObject Clone()
        {
            return CloneImplementation();
        }

        /// <summary>
        /// Remove this element from the DOM
        /// </summary>
        public virtual void Remove()
        {
            if (ParentNode == null)
            {
                throw new InvalidOperationException("This element has no parent.");
            }
            ParentNode.ChildNodes.Remove(this);
        }

        public virtual int DescendantCount()
        {
            return 0;
        }

        public virtual void AppendChild(IDomObject element)
        {
            throw new InvalidOperationException("This type of element does not have children.");
        }
        public virtual void RemoveChild(IDomObject element)
        {
            throw new InvalidOperationException("This type of element does not have children.");
        }
        public virtual void InsertBefore(IDomObject newNode, IDomObject referenceNode)
        {
            throw new InvalidOperationException("This type of element does not have children.");
        }
        public virtual void InsertAfter(IDomObject newNode, IDomObject referenceNode)
        {
            throw new InvalidOperationException("This type of element does not have children.");
        }

        public virtual void SetAttribute(string name)
        {
            throw new InvalidOperationException("You can't set attributes for this element type.");
        }

        public virtual void SetAttribute(string name, string value)
        {
            throw new InvalidOperationException("You can't set attributes for this element type.");
        }

        public virtual string GetAttribute(string name)
        {
            return null;
        }

        public virtual string GetAttribute(string name, string defaultValue)
        {
            return null;
        }

        public virtual bool TryGetAttribute(string name, out string value)
        {
            value = null;
            return false;
        }
        public virtual bool HasAttribute(string name)
        {
            return false;
        }

        public virtual bool RemoveAttribute(string name)
        {
            throw new InvalidOperationException("You can't remove attributes from this element type.");
        }

        public virtual bool HasClass(string className)
        {
            return false;
        }

        public virtual bool AddClass(string className)
        {
            throw new InvalidOperationException("You can't add classes to this element type.");
        }

        public virtual bool RemoveClass(string className)
        {
            throw new InvalidOperationException("You can't remove classes from this element type.");
        }

        public virtual bool HasStyle(string styleName)
        {
            return false;
        }

        public virtual void AddStyle(string styleString)
        {
            throw new InvalidOperationException("You can't add styles to this element type.");
        }

        public virtual bool RemoveStyle(string name)
        {
            throw new InvalidOperationException("You can't remove styles to this element type.");
        }


        public override string ToString()
        {
            return Render();
        }
        #endregion

        #region private methods

        protected string PathEncode(int number)
        {
            // return number.ToString().PadLeft(pathIdLength, '0');
            return HtmlParser.HtmlData.BaseXXEncode(number);
        }

        //public override int GetHashCode()
        //{
        //    return Depth * Index;
        //}

        //public override bool Equals(object obj)
        //{
        //    return ReferenceEquals(this, obj);
        //}

        #endregion

        #region interface members

         IDomNode IDomNode.Clone()
         {
             return Clone();
         }
         object ICloneable.Clone()
         {
             return Clone();
         }

        #endregion
        
    }

}