using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using mapApi.classes;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Collections.ObjectModel;

namespace mapApi
{
    public partial class MainWindow : Window
    {
        // 클래스 멤버로 ObservableCollection 선언(바인딩할 때 사용)
        private ObservableCollection<MyLocale> favoriteList = new ObservableCollection<MyLocale>();
        private FavoriteRepository favoriteRepo = new FavoriteRepository();

        public MainWindow()
        {
            InitializeComponent();
            lbox_favorite.ItemsSource = favoriteList; // 즐겨찾기 리스트 바인딩
            LoadFavoritesFromDb();
        }

        private void LoadFavoritesFromDb()
        {
            favoriteList.Clear();
            foreach (var fav in favoriteRepo.GetAllFavorites())
            {
                favoriteList.Add(fav);
            }
        }

        // 검색 버튼
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<MyLocale> mls = KakaoApi.Search(tbox_query.Text);
            lbox_locale.ItemsSource = mls; // 리스트박스 내용 변경
        }

        // 리스트에서 장소 클릭
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbox_locale.SelectedIndex == -1) { return; }

            MyLocale ml = lbox_locale.SelectedItem as MyLocale;
            object[] ps = new object[] { ml.Lat, ml.Lng };
						
			// c#에서 javascript 코드를 부르는 함수
            wb.InvokeScript("setCenter", ps);
        }
				
		// 5개 지역으로 표시
		// 구역별 라디오 버튼 클릭 시 지도 중심 이동
        private void AreaButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                string area = radioButton.Tag.ToString();

                (double lat, double lng) = area switch
                {
                    "사대부고" => (35.846125, 127.136224),
                    "덕진 광장" => (35.844603, 127.124466),
                    "구정문 카페 거리" => (35.842209, 127.128159),
                    "덕진초" => (35.841463, 127.124717),
                    "할리스 건물 뒷편" => (35.841042, 127.134832),
                    _ => throw new InvalidOperationException("알 수 없는 구역입니다.")
                };

                object[] ps = new object[] { lat, lng };
                wb.InvokeScript("setCenter", ps);
            }
        }
        
        // 카테고리 선택 - ListBox 선택 변경 시
		private void Category_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{   
            // category.selectedItem이 ListBoxItem 타입일 경우 selectedItem 이라고 칭함
		    if (Category.SelectedItem is ListBoxItem selectedItem && selectedItem.Tag != null)
		    {
		        string category = selectedItem.Tag.ToString();
		        object[] args = { category };
		        wb.InvokeScript("searchCategory", args);
		    }
		}
        // 팁 내용
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("1. 층수와 채광, 통풍 2. 주변 환경 및 편의시설 3. 계약 조건 및 관리비 추가 내역");
        }

        // 즐겨찾기 추가 버튼 클릭 이벤트
        private void btn_favorite_Click(object sender, RoutedEventArgs e)
        {
            if (lbox_locale.SelectedItem is MyLocale selected)
            {
                // 중복 방지: 이미 즐겨찾기에 있으면 추가하지 않음
                if (!favoriteList.Contains(selected))
                {
                    favoriteRepo.AddFavorite(selected); // DB에 저장
                    favoriteList.Add(selected);         // UI에 추가
                }
                else
                {
                    MessageBox.Show("이미 즐겨찾기에 추가된 항목입니다.");
                }
            }
            else
            {
                MessageBox.Show("추가할 검색 결과를 선택하세요.");
            }
        }

        private void lbox_favorite_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(lbox_favorite.SelectedItem is MyLocale selected)
            {
                object[] ps = new object[] { selected.Lat, selected.Lng };
                wb.InvokeScript("setCenter", ps);
            }
        }
    }
}
