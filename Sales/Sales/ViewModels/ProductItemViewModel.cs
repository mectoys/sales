﻿
namespace Sales.ViewModels
{
    using System;
    using System.Linq;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Sales.Common.Models;
    using Sales.Helpers;
    using Services;
    using Views;

    using Xamarin.Forms;

    public class ProductItemViewModel : Product
    {
        //los modelos deben de tener propiedades , no pueden tener metodos

        #region Atributes
        private ApiService apiService;

        #endregion

        #region Constructor
        public ProductItemViewModel()
        {
            this.apiService = new ApiService();
        }

        #endregion

        #region Commands
        public ICommand  EditProductCommand
        {
            get
            {
                return new RelayCommand(EditProduct);
            }
        }

        private async void EditProduct()
        {
            //Creamos unsiglento de la MainViewModel
            //crear la instacia y ligar con la mainviewmodel
            MainViewModel.GetInstance().EditProduct = new EditProductViewModel(this);

            await  App.Navigator.PushAsync(new EditProductPage());
        }

        public ICommand DeleteProductCommand {
            get {
                return new RelayCommand(DeleteProduct);
            }
        }

        private  async void DeleteProduct()
        {
            var answer =await  Application.Current.MainPage.DisplayAlert(
                Languages.Confirm,
                Languages.DeleteConfirmation,
                Languages.Yes, Languages.No);
            if (!answer)
            {
                return;

            }
            var connection = await this.apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                 
                await Application.Current.MainPage.DisplayAlert(Languages.Error, connection.Message, Languages.Accept);
                return;
            }
            //agregar al diccionarion de recursos ap.xaml la direccion URL por seguridad
            var url = Application.Current.Resources["UrlAPI"].ToString();
            var prefix = Application.Current.Resources["UrlPrefix"].ToString();
            var controller = Application.Current.Resources["UrlProductsController"].ToString();

            var response = await this.apiService.Delete(url, prefix, controller,this.ProductId, Settings.TokkenType, Settings.AccessToken);
            //aca devolvio una lista de obj. response
            if (!response.IsSuccess)
            {                
                await Application.Current.MainPage.DisplayAlert(Languages.Error, response.Message, Languages.Accept);
                return;
            }
            //refrescar la lista
            //llamar el singleton
            var productsViewModel = ProductsViewModel.GetInstance();
            var deletedProduct = productsViewModel.MyProducts.Where(p => p.ProductId == this.ProductId).FirstOrDefault();
            if (deletedProduct !=null)
            {
                productsViewModel.MyProducts.Remove(deletedProduct);
            }
            productsViewModel.RefreshList();
        }
        #endregion
    }
}
