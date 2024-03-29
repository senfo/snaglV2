﻿//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Berico.SnagL.Infrastructure.Clustering;
using Berico.SnagL.Infrastructure.Events;
using Berico.SnagL.Infrastructure.Modularity.Contracts;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using BERICO = Berico.Windows.Controls;

namespace Berico.SnagL.Infrastructure.Modularity.Toolbar
{
	[PartMetadata("ID", "ToolbarItemViewModelExtension"), Export(typeof(TreeToolbarItemExtensionViewModel))]
	public class TreeToolbarItemExtensionViewModel : ViewModelBase, IToolbarItemViewModelExtension
	{
		private int index = 0;
		private string description = string.Empty;
		private bool isEnabled = true;

		private BitmapImage captionImage;
		private string searchToolMode;
		private ObservableCollection<BERICO.MenuItem> menuItems;
		private BERICO.MenuItem selectedMenuItem;

		public ObservableCollection<BERICO.MenuItem> MenuItems
		{
			get
			{
				return menuItems;
			}
		}

		public BERICO.MenuItem SelectedMenuItem
		{
			get
			{
				return selectedMenuItem;
			}
			set
			{
				selectedMenuItem = value;
				RaisePropertyChanged("SelectedItem");
			}
		}


		/// <summary>
		/// Gets the command that should be executed when an item
		/// in the menu is selected
		/// </summary>
		public ICommand SelectionChangedCommand
		{
			get
			{
				return new RelayCommand<SelectionChangedEventArgs>(e =>
				{
					// Cast the object that fired the event
					BERICO.MenuItem selectedMenuItem = e.AddedItems[0] as BERICO.MenuItem;

					// Set the image of the search button to the image for
					// the selected menu item
					CaptionImage = (selectedMenuItem.Icon as Image).Source as BitmapImage;

					// Set the tool mode accordingly
					searchToolMode = selectedMenuItem.Tag.ToString();
					if (searchToolMode == "Tree")
					{
						this.Name = "TREE_LAYOUT";
					}
					else // searchToolMode == "Simple Tree"
					{
						this.Name = "SIMPLE_TREE_LAYOUT";
					}
				});
			}
		}

		public TreeToolbarItemExtensionViewModel()
		{
			index = 21;
			description = "Tree Layout:  Layout the graph in a tree pattern";
			Name = "TREE_LAYOUT";

			SnaglEventAggregator.DefaultInstance.GetEvent<Clustering.ClusteringCompletedEvent>().Subscribe(ClusteringCompletedEventHandler, false);

			menuItems = new ObservableCollection<BERICO.MenuItem>();

			BitmapImage treeImage = new BitmapImage(new Uri("/Berico.SnagL;component/Resources/Icons/SnagL/Layout_Tree.png", UriKind.Relative));
			BitmapImage simpleTree = new BitmapImage(new Uri("/Berico.SnagL;component/Resources/Icons/SnagL/Layout_Tree.png", UriKind.Relative));
			BERICO.MenuItem menuItem;

			menuItem = new BERICO.MenuItem()
			{
				Header = "Tree",
				Tag = "Tree",
				Icon = new Image()
				{
					Source = treeImage,
					Height = 15,
					Width = 15,
					Margin = new Thickness(2)
				}
			};
			MenuItems.Add(menuItem);

			this.selectedMenuItem = menuItem;
			this.CaptionImage = treeImage;
			this.searchToolMode = "Tree";

			menuItem = new BERICO.MenuItem()
			{
				Header = "Simple Tree",
				Tag = "Simple Tree",
				Icon = new Image()
				{
					Source = simpleTree,
					Height = 15,
					Width = 15,
					Margin = new Thickness(2)
				}
			};
			MenuItems.Add(menuItem);
		}

		/// <summary>
		/// Gets or sets the Icon that is used as the search button's
		/// caption
		/// </summary>
		public BitmapImage CaptionImage
		{
			get
			{
				return this.captionImage;
			}
			set
			{
				this.captionImage = value;
				RaisePropertyChanged("CaptionImage");
			}
		}

		/// <summary>
		/// Handles the ClusteringCompleted event
		/// </summary>
		/// <param name="args">The arguments for the event</param>
		public void ClusteringCompletedEventHandler(ClusteringCompletedEventArgs args)
		{
			IsEnabled = !args.ClusteringActive;
		}

		protected virtual void OnToolbarItemSelected(EventArgs e)
		{
			if (ToolbarItemSelected != null)
			{
				foreach (IToolbarItemViewExtension ve in ToolbarExtensionManager.Instance.Extensions)
				{
					IToolbarItemViewModelExtension radioButtonVM = ve.ViewModel;
					if (radioButtonVM.Name.EndsWith("_LAYOUT") && radioButtonVM != this)
					{
						Type radioButtonType = radioButtonVM.GetType();
						PropertyInfo isCheckedProp = radioButtonType.GetProperty("IsChecked");
						isCheckedProp.SetValue(radioButtonVM, false, null);
					}
				}

				ToolbarItemSelected(this, e);
			}
		}

		/// <summary>
		/// Gets or sets the currently selected item.  This value is bound
		/// directly to the view.
		/// </summary>
		public BERICO.MenuItem SelectedItem
		{
			get
			{
				return this.selectedMenuItem;
			}
			set
			{
				this.selectedMenuItem = value;
				RaisePropertyChanged("SelectedItem");
			}
		}

		#region IToolbarItemViewModelExtension Members

		public event EventHandler<EventArgs> ToolbarItemSelected;

		public int Index
		{
			get
			{
				return index;
			}

			set
			{
				index = value;
			}
		}

		public string Name
		{
			get;
			set;
		}

		public string Description
		{
			get
			{
				return description;
			}

			set
			{
				description = value;
				RaisePropertyChanged("Description");
			}
		}

		public bool IsEnabled
		{
			get
			{
				return isEnabled;
			}

			set
			{
				isEnabled = value;
				RaisePropertyChanged("IsEnabled");
			}
		}

		public ICommand ItemSelected
		{
			get
			{
				return new RelayCommand(() =>
				{
					OnToolbarItemSelected(EventArgs.Empty);
				});
			}
		}

		#endregion
	}
}