   M S B A M L   `   `   `  ����W  SInverted_Index_Campbell_Reid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullK*clr-namespace:Inverted_Index_Campbell_ReidInverted_Index_Campbell_Reid  5   	   -    'Inverted_Index_Campbell_Reid.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   <local*clr-namespace:Inverted_Index_Campbell_Reid     5   	     � Title$  
MainWindow��5   	   $	��762��6   $	��941��6)   .��5	      � .+�5
   
   �� -   5         ��Name$ rootFolderDisplay��5
      $
��Left=�6-   $��20��6H   $��	25,40,0,0q�6T    �� TextWrapping$
 Wrap=�6g   $	��Top=�6{   $	��330��6�    �� 
IsReadOnly . 6�   �� 5   
   -   5         ��Name$ selectFolderButton��5      $��Select Folder��6-   $
��Left=�6E   $��
385,40,0,0q�6`   $	��Top=�6t   $��90��6�   6�   �� 5   
   -   5        $ searchParams��5      $
��Left=�6(   $��20��6C   $��
25,100,0,0q�6O   $
 Wrap=�6c   $	��Top=�6w   $	��330��6�   �� 5   
   -   5        $ searchButton��5      $��Search��6'   $
��Left=�68   $��385,100,0,0q�6S   $	��Top=�6h   $��90��6�   6�   �� 5   
   -   5         ��Name$ listBox��5      $
��Left=�6#   $	��580��6>   $��
25,141,0,0q�6K   $	��Top=�6_   $	��879��6w   �� 5   
   -   5         ��Name$ synonymsBox��5      $��Search for synonyms��6(   $
��Left=�6F   $��
518,40,0,0q�6a   $	��Top=�6u    d� 	IsChecked$
 TrueY�6�   �� 5   
   -   5        $ stemmingBox��5      $��
Stem words��6(   $
��Left=�6=   $��
518,70,0,0q�6X   $	��Top=�6l   $
 TrueY�6�   	� 5   
   -   5         	�Name$ 
and_Button��5      $	��AND��6*   $
��Left=�68   $��
803,40,0,0q�6S   $	��Top=�6g   	 	� 	GroupName$	 sType��6   $
 TrueY�6�   �� 5   
   -	   5        
 ��Name$
 Search_type��5      $
��Left=�6)   $��
711,38,0,0q�6D   $
��Wrap=�6X   $��Search type:��6l   $	��Top=�6�   	� 5   
   -
   5        $ 	or_Button��5      $��OR��6)   $
��Left=�66   $��
803,70,0,0q�6Q   $	��Top=�6e   $	 sType��6}   	� 5   
   -   5        $	 xor��5      $	��XOR��6#   $
��Left=�61   $��803,100,0,0q�6L   $	��Top=�6a   $	 sType��6y   5      5      