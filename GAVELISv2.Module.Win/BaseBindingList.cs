using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;
using System.Reflection;
namespace GAVELISv2.Module.Win {
    public class BaseBindingList<T> : BindingList<T> {
        #region Private variables for Searching
        private ArrayList selectedIndices;
        private int[] returnIndices;
        #endregion
        #region Private variables for Sorting
        private bool isSortedValue;
        private ListSortDirection sortDirectionValue;
        private PropertyDescriptor sortPropertyValue;
        private ArrayList sortedList;
        private ArrayList unsortedList;
        #endregion
        /// <summary>
        /// By default this property is set to false
        /// Set this property to true to indicate searching is implemented on list.
        /// </summary>
        #region Properties for Searching
        protected override bool SupportsSearchingCore { get { return true; } }
        #endregion
        #region Properties for Sorting
        protected override PropertyDescriptor SortPropertyCore { get { return 
                sortPropertyValue; } }
        protected override ListSortDirection SortDirectionCore { get { return 
                sortDirectionValue; } }
        protected override bool SupportsSortingCore { get { return true; } }
        protected override bool IsSortedCore { get { return isSortedValue; } }
        #endregion
        public BaseBindingList() { }
        #region Methods for searching
        protected override int FindCore(PropertyDescriptor prop, object key) {
            // Get the property info for the specified property.
            PropertyInfo propInfo = typeof(T).GetProperty(prop.Name);
            T item;
            int found = -1;
            selectedIndices = new ArrayList();
            if (key != null) {
                // Loop through the items to see if the key
                // value matches the property value.
                for (int i = 0; i < Count; ++i) {
                    item = (T)Items[i];
                    if (propInfo.GetValue(item, null).Equals(key)) {
                        found = 0;
                        selectedIndices.Add(i);
                    }
                }
            }
            return found;
        }
        /// <summary>
        /// This method returns the indices of the business objects in the list based on the search criteria
        /// </summary>
        /// <param name="property">Name of the property of the Business Object</param>
        /// <param name="key">Value to be search with</param>
        /// <returns></returns>
        public int[] Find(string property, object key) {
            // Check the properties for a property with the specified name.
            PropertyDescriptorCollection properties = TypeDescriptor.
            GetProperties(typeof(T));
            PropertyDescriptor prop = properties.Find(property, true);
            // If there is not a match, return -1 otherwise pass search to
            // FindCore method.
            if (prop == null) {returnIndices = null;} else {
                if (FindCore(prop, key) >= 0) {returnIndices = (int[])(
                    selectedIndices.ToArray(typeof(int)));}
            }
            return returnIndices;
        }

        /// <summary>
        /// This method returns the indices of the business objects in the list based on the search criteria
        /// </summary>
        /// <param name="property">Name of the property of the Business Object</param>
        /// <param name="key">Value to be search with</param>
        /// <returns></returns>
        public int[] Find2(string property, object key)
        {
            // Check the properties for a property with the specified name.
            PropertyDescriptorCollection properties = TypeDescriptor.
            GetProperties(typeof(T));
            PropertyDescriptor prop = properties.Find(property, true);
            // If there is not a match, return -1 otherwise pass search to
            // FindCore method.
            if (prop == null) { returnIndices = null; }
            else
            {
                if (FindCore(prop, key) >= 0)
                {
                    returnIndices = (int[])(
                        selectedIndices.ToArray(typeof(int)));
                }
                else
                {
                    returnIndices = null;
                }
            }
            return returnIndices;
        }
        #endregion
        #region Methods for Sorting
        protected override void ApplySortCore(PropertyDescriptor prop, 
        ListSortDirection direction) {
            sortedList = new ArrayList();
            // Check to see if the property type we are sorting by implements
            // the IComparable interface.
            Type interfaceType = prop.PropertyType.GetInterface("IComparable");
            if (interfaceType != null) {
                // If so, set the SortPropertyValue and SortDirectionValue.
                sortPropertyValue = prop;
                sortDirectionValue = direction;
                if (!isSortedValue) {unsortedList = new ArrayList(this.Count);}
                // Loop through each item, adding it the the sortedItems ArrayList.
                foreach (Object item in this.Items) {
                    sortedList.Add(prop.GetValue(item));
                    //Make sure that unsorted list keeps the original value when sorting is applied for the first time
                    if (!isSortedValue) {unsortedList.Add(item);}
                }
                // Call Sort on the ArrayList.
                sortedList.Sort();
                // Check the sort direction and then copy the sorted items
                // back into the list.
                if (direction == ListSortDirection.Descending) {sortedList.
                    Reverse();}
                for (int i = 0; i < this.Count; i++) {
                    int[] selectedIndices = this.Find(prop.Name, sortedList[i]);
                    if (selectedIndices != null && selectedIndices.Length > 0) {
                        foreach (int position in selectedIndices) {if (position 
                            != i) {SwapItems(i, position);}}}
                }
                isSortedValue = true;
                // Raise the ListChanged event so bound controls refresh their
                // values.
                OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1
                ));
            } else {
                // If the property type does not implement IComparable, let the user
                // know.
                throw new NotSupportedException("Cannot sort by " + prop.Name + 
                ". This" + prop.PropertyType.ToString() + 
                " does not implement IComparable");
            }
        }
        protected override void RemoveSortCore() {
            // Ensure the list has been sorted.
            if (unsortedList != null) {
                // Loop through the unsorted items and reorder the
                // list per the unsorted list.
                for (int i = 0; i < unsortedList.Count; i++) {this[i] = (T)
                    unsortedList[i];}
                isSortedValue = false;
                OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1
                ));
            }
        }
        public void RemoveSort() { RemoveSortCore(); }
        private void SwapItems(int fromIndex, int toIndex) {
            T temp = this[fromIndex];
            this[fromIndex] = this[toIndex];
            this[toIndex] = temp;
        }
        public override void EndNew(int itemIndex) {
            // Check to see if the item is added to the end of the list,
            // and if so, re-sort the list.
            if (sortPropertyValue != null && itemIndex == this.Count - 1) {
                ApplySortCore(this.sortPropertyValue, this.sortDirectionValue);}
            base.EndNew(itemIndex);
        }
        public void ApplySort(string property, ListSortDirection direction) {
            PropertyDescriptorCollection properties = TypeDescriptor.
            GetProperties(typeof(T));
            PropertyDescriptor prop = properties.Find(property, true);
            if (prop != null) {ApplySortCore(prop, direction);} else {
                throw new NotSupportedException("Cannot sort by " + prop.Name + 
                ". This" + prop.Name + " does not exist.");
            }
        }
        #endregion
    }
}
