   M S B A M L   `   `   `  ����J  FBlockSocNetwork, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:BlockSocNetworkBlockSocNetwork  5   	   $    BlockSocNetwork.PasswordWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:BlockSocNetwork     5   	   -   5          �Name$  
PswdWindow��5   	    � WindowStartupLocation$ CenterScreen=�5	   	    � Title$ Вход��5   	   $	��180��6   $	��350��6#   $	��190��6/   $	��350��6?   $	��190��6N   $	��350��6^    � Icon$ 	Block.ico��6m    � 	Resources 5       �� 
TargetType   ButtonStyle �� Property �� Value �� 
TargetType	 �� Triggers
 P� Property P� Value �� 
TargetName��%�  &	        �� 5   
   " ��6   .�5      ��  � �� 5      �� 5      " ��6&   .R�5      �� $��Border��6!   $��1q�6/   ���#cdcdcd._�5      �� $��Center=�6/   $��Center=�6J   $��Content��6g   5      	 5      P� 5      
 �@J�IsMouseOver . . �5   "   �� $ Border��6)    � �� �#f2f2f26S   5      P� 5      
 �@J�IsMouseOver .  . �5   "   �� $ Border��6)    � �� �#e0e0e06S   5      5      5      5      5       .��5!      �  � RowDefinitions 5"   
   �� 5#      $
��Auto �6   �� 5$      $
��Auto �6   �� 5%      $
��Auto �6   �� 5&      $
��Auto �6   .+�5(   
   �� -   5        $��Error��5(      $��0��6!   
���  ��$��Center=�6?   $��Center=�6Z   $��WrapWithOverflow=�5)      $+��%Ошибка ввода пароля!��64   $��0 5 0 0q�6P   $y�Hidden=�6a   �� 5*   
   -   5        $��
FirstEnter��5*      $��1��6'   $��Center=�64   $��Center=�6P   $
��0 10q�6n   $y�Hidden=�6|   .+�5+      �� $��WrapWithOverflow=�6   $?��9Программа запускается впервые.��68   �� 5,      $��WrapWithOverflow=�6   $:��4Придумайте и введите пароль.��68   5-      �� 5.   
   -   5        $��Enter��5.      $��1��6!   $��Center=�6.   $��Center=�6I   $��WrapWithOverflow=�6f   $
��0 10q�5/      $3��-Введите пароль для входа��6"   $y�Hidden=�6B   � 50   
   $��2��6   $��Center=�6!   $��Center=�6>   $	��0 5q�6Y   .+�51      �� $��Пароль ��6   G� 52      -   5        $��Password��52      $	��100��6+   53      �� 54   
   -   5        $��3��54      $��Center=�6   $��Center=�69   $��60��6V   $��25��6a   $
��0 10q�55      $��
Войти��6   #��[  6D   56      57      