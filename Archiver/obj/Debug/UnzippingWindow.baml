   M S B A M L   `   `   `  ����C  ?Archiver, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:ArchiverArchiver  5   	       Archiver.UnzippingWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:Archiver     5   	   -   5          � Title$  Разархивация��5   	   $��258.834��6   $��730.639��6/    � 
ResizeMode$ CanMinimize=�6?   .��5	      � .+�5
   
   �� -   5         ��Name$ ChooseButton��5
      $��Выбрать архив��6'   $��579,0,10,186q�6?   $��34��6U   $��Bottom=�6|   �� 5   
   -   5         ��Name$ ChooseTextBox��5       �� 
IsReadOnly . $��34��6;   $��10,0,169,186q�6G    �� TextWrapping$
 Wrap=�6]   $�� ��6q   $��Bottom=�6y   �� 5   
   $��Пароль:��6   $��262,0,405,151q�6"   $��26��69   $��Bottom=�6E   �� 5   
   -   5        $ UnzipButton��5      $$��Разархивировать��6&   $��579,0,10,147q�6@   $��34��6p   $��Bottom=�6|   G� 5   
   -   5         G�Name$ PasswordBox��5      $��340,0,169,147q�6+   $��26��6B   $��Bottom=�6N   �� 5   
   -   5         ��Name$ 
LogListBox��5      $��
10,0,10,10q�6&   $	��122��6:   $��Bottom=�6G   5      5      