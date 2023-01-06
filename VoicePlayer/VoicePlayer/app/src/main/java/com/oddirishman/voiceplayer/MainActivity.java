package com.oddirishman.voiceplayer;

import android.Manifest;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.content.pm.ResolveInfo;
import android.os.Bundle;
import android.speech.RecognizerIntent;
import android.support.v4.app.ActivityCompat;
import android.support.v4.content.ContextCompat;
import android.support.v7.app.AppCompatActivity;
import android.view.Gravity;
import android.view.View;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.ListView;
import android.widget.Toast;

import java.io.IOException;
import java.io.ObjectOutputStream;
import java.io.OutputStream;
import java.net.InetAddress;
import java.net.NetworkInterface;
import java.net.Socket;
import java.net.SocketException;
import java.util.ArrayList;
import java.util.Enumeration;
import java.util.List;

public class MainActivity extends AppCompatActivity implements View.OnClickListener {

    private static final int VR_REQUEST=999; //номер, который потом понадобится при распознавании
    private ListView wordList; //список, в котороый будем кидать распознанное
    private OutputStream os; //объект, который будет отправлять сообщения другому сокету т.е. плееру
    private Socket socket = null; //сокет
    private Thread th; //поток
    private String mes = ""; //строка с сообщением

    @Override
    protected void onCreate(Bundle savedInstanceState) { //при создании
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main); //стандартные инициализаторы студии

        int permissionStatus = ContextCompat.checkSelfPermission(this, Manifest.permission.INTERNET);

        if (permissionStatus != PackageManager.PERMISSION_GRANTED) {
            ActivityCompat.requestPermissions(this, new String[] {Manifest.permission.INTERNET},
                    13);
        }
        //теперь стоит объяснить, в яве ты не можешь нормально обращаться к объектам приложения (кнопкам и пр.) напрямую
        //т.е. как в c# button1.Text и тд.
        //здесь приходится создавать объект нужного типа в коде и делать так, чтобы он ссылался на объект в приложении
        //вот пример, в приложении есть кнопка speech_btn и чтобы работать с ней, мы создаем объект типа Кнопка и кладем
        // туда нашу кнопку из приложения, находя ее по ID кнопки.
        Button speechBtn=(Button) findViewById(R.id.speech_btn);
        wordList=(ListView) findViewById(R.id.word_list); //то же самое со списком сообщений (listbox в c#)
        //далее мы проверяем можем ли мы вообще распознавать речь на этом телефоне
        PackageManager packManager = getPackageManager();
        List<ResolveInfo> intActivities= packManager.queryIntentActivities(new
                Intent(RecognizerIntent.ACTION_RECOGNIZE_SPEECH),0);
        if(intActivities.size()!=0){ // если список не пуст, то устанавливаем на нашу кнопку событие this (т.е. событие OnClick)
                                     // которое описано ниже. this означает, что в этом классе
            speechBtn.setOnClickListener(this);
        }
        else
        { //если все же нельзя распознавать, то кнопку делаем неактивной т.е. не сможем нажать и выводим сообщение на экран
            speechBtn.setEnabled(false);
            Toast.makeText(this,"Oops - Speech recognition not supported!", Toast.LENGTH_LONG).show();
        }
                try { //пытаемся подключится
                    connect();
                }
                catch (Exception ex){

                }
    }

    private void connect() { //подключение
        th = new Thread(new Runnable() { //создаем новй поток
            @Override
            public void run() { //который при старте выполнит эту функцию
                try { //мы пытаемся подключится к сокету
                    if(socket != null) //если сокет уже подключен, то вырубаем его (вдруг повторное подключение)
                        socket.close();
                    socket = new Socket("192.168.43.189", 1313); //и подключаемся к сокету по этому айпи и порту
                    //этот айпи будет айпи локальный твоего компа, если что посмотри его на своем компе с помощью ipconfig в cmd
                    os = socket.getOutputStream(); // инициализируем отправляку сообщений
                    while (true) { //и потом в бесконечном цикле
                        if(!socket.isConnected()) { //подключен ли еще сокет
                            Thread.currentThread().interrupt(); //если нет, то завершаем текущий поток
                            return;
                        }
                        if(mes != "") { //проверяем, если сообщение не пустое
                            String toSend = mes; //то в отправляемое сообщение запихиваем mes
                            byte[] toSendBytes = toSend.getBytes(); //получаем массив байтов сообщения
                            os.write(toSendBytes); //отправляем его
                            mes = ""; //делаем сообщение пустым
                        }
                    }
                }
                catch (IOException ex){ //если не получаеся. то показываем ошибку
                    Toast toast3 = Toast.makeText(getApplicationContext(),
                            ex.getMessage(), Toast.LENGTH_LONG);
                    toast3.setGravity(Gravity.CENTER, 0, 0);
                    toast3.show();
                }
            }
        });
        th.start(); //а и поток надо запустить
    }

    private void listenToSpeech(){

//запускаем интент, распознающий речь и передаем ему требуемые данные
        Intent listenIntent=new Intent(RecognizerIntent.ACTION_RECOGNIZE_SPEECH);
//указываем пакет
        listenIntent.putExtra(RecognizerIntent.EXTRA_CALLING_PACKAGE,
                getClass().getPackage().getName());
//В процессе распознования выводим сообщение
        listenIntent.putExtra(RecognizerIntent.EXTRA_PROMPT,"Say a word!");
//устанавливаем модель речи
        listenIntent.putExtra(RecognizerIntent.EXTRA_LANGUAGE_MODEL,
                RecognizerIntent.LANGUAGE_MODEL_FREE_FORM);
//указываем число результатов, которые могут быть получены
        listenIntent.putExtra(RecognizerIntent.EXTRA_MAX_RESULTS,10);

//начинаем прослушивание
        startActivityForResult(listenIntent, VR_REQUEST);
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data){
        if(requestCode== VR_REQUEST && resultCode== RESULT_OK)
        {
            ArrayList<String> suggestedWords=
                    data.getStringArrayListExtra(RecognizerIntent.EXTRA_RESULTS);
            wordList.setAdapter(new ArrayAdapter<String>(this, R.layout.word, suggestedWords));
            //здесь все просто, мы смотрим результат распознавания записываем в mes сообщение для отправки
            //а код выше каждый раз проверяет есть ли сообщение для отправки
            if(suggestedWords.contains("котики")) //если скажешь котики, то вызовется метод, что ниже
                showToast();
            if(suggestedWords.contains("громче"))
                mes = "Громче";
            else if(suggestedWords.contains("тише"))
                mes = "Тише";
            else if(suggestedWords.contains("следующий"))
                mes = "Следующий";
            else if(suggestedWords.contains("предыдущий"))
                mes = "Предыдущий";
            else if(suggestedWords.contains("стоп"))
                mes = "Стоп";
            else if(suggestedWords.contains("играть"))
                mes = "Играть";
            else if(suggestedWords.contains("звук"))
                mes = "Звук";
            else if(suggestedWords.contains("ещё"))
                mes = "Ещё";
            else if(suggestedWords.contains("открыть"))
                mes = "Открыть";
            else{
                String str = suggestedWords.get(0); //тут мы отправляем сообщение с названием песни
                for(int i = 1; i<suggestedWords.size(); i++)
                    str = str + "|" + suggestedWords.get(i); //составляем строчку со всеми вариантами, что мы распознали
                mes = str.toLowerCase(); //и в нижний регистр переписываем, чтобы потом в плеере легче распознать
            }
        }
        super.onActivityResult(requestCode, resultCode, data);
    }

    public void onClick(View v){ //событие нажатия
        if(v.getId()== R.id.speech_btn){ //если нажата кнопка распознавания с экрана
            listenToSpeech(); //начинаем распознавать
        }
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) { //этот и следующий методы стандартные
        getMenuInflater().inflate(R.menu.menu_main, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        int id = item.getItemId();
        if (id == R.id.action_settings) {
            return true;
        }

        return super.onOptionsItemSelected(item);
    }

    public void showToast() { //наш котик)
        Toast toast3 = Toast.makeText(getApplicationContext(),
                "Спасибо, что воспользовались нашими услугами ♥", Toast.LENGTH_LONG);
        toast3.setGravity(Gravity.CENTER, 0, 0);
        LinearLayout toastContainer = (LinearLayout) toast3.getView();
        ImageView catImageView = new ImageView(getApplicationContext());
        catImageView.setImageResource(R.drawable.cat);
        catImageView.setScaleType(ImageView.ScaleType.FIT_CENTER);
        toastContainer.addView(catImageView, 0);
        toast3.show();
    }

}
